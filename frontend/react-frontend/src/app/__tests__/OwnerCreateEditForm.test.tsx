import React from 'react'
import { render, screen, waitFor, act } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import OwnerCreateEditForm from '../components/Owners/OwnerCreateEditForm'

// Mock the API module
jest.mock('../components/Api', () => ({
  fetchOwnerById: jest.fn()
}))

// Mock the Translations component
jest.mock('../components/Translations', () => {
  return function T(key: string) {
    const translations: { [key: string]: string } = {
      'owner': 'Owner',
      'firstName': 'First Name',
      'lastName': 'Last Name',
      'address': 'Address',
      'city': 'City',
      'telephone': 'Telephone',
      'save': 'Save',
      'cancel': 'Cancel',
      'requiredField': 'This field is required',
      'addOwner': 'Add Owner',
      'editOwner': 'Edit Owner'
    }
    return translations[key] || key
  }
})

// Mock fetch
global.fetch = jest.fn()

describe('OwnerCreateEditForm', () => {
  const mockSetHash = jest.fn()
  /* eslint-disable @typescript-eslint/no-require-imports */
  const { fetchOwnerById } = require('../components/Api')
  
  let consoleWarnSpy: jest.SpyInstance
  let consoleErrorSpy: jest.SpyInstance
  
  beforeEach(() => {
    mockSetHash.mockClear()
    fetchOwnerById.mockClear()
    ;(global.fetch as jest.Mock).mockClear()
    
    // Suppress console warnings and errors by default to reduce test noise
    consoleWarnSpy = jest.spyOn(console, 'warn').mockImplementation(() => {})
    consoleErrorSpy = jest.spyOn(console, 'error').mockImplementation(() => {})
  })

  afterEach(() => {
    consoleWarnSpy.mockRestore()
    consoleErrorSpy.mockRestore()
  })

  describe('Create Mode', () => {
    const createModeProps = {
      hash: '#owners/new',
      setHash: mockSetHash
    }

    test('renders create form correctly', async () => {
      await act(async () => {
        render(<OwnerCreateEditForm {...createModeProps} />)
      })
      
      expect(screen.getByLabelText('First Name')).toBeDefined() // First name input
      expect(screen.getByText('First Name')).toBeDefined()
      expect(screen.getByText('Last Name')).toBeDefined()
      expect(screen.getByText('Address')).toBeDefined()
      expect(screen.getByText('City')).toBeDefined()
      expect(screen.getByText('Telephone')).toBeDefined()
      expect(screen.getByRole('button', { name: 'Add Owner' })).toBeDefined()
    })

    test('allows user to enter owner information', async () => {
      const user = userEvent.setup()
      await act(async () => {
        render(<OwnerCreateEditForm {...createModeProps} />)
      })
      
      const inputs = screen.getAllByDisplayValue('')
      const [firstNameInput, lastNameInput, addressInput, cityInput, telephoneInput] = inputs
      
      await user.type(firstNameInput, 'John')
      await user.type(lastNameInput, 'Doe')
      await user.type(addressInput, '123 Main St')
      await user.type(cityInput, 'Springfield')
      await user.type(telephoneInput, '555-0123')
      
      expect(firstNameInput).toHaveValue('John')
      expect(lastNameInput).toHaveValue('Doe')
      expect(addressInput).toHaveValue('123 Main St')
      expect(cityInput).toHaveValue('Springfield')
      expect(telephoneInput).toHaveValue('555-0123')
    })

    test('submits form with correct data', async () => {
      const user = userEvent.setup()
      const mockFetch = global.fetch as jest.Mock
      mockFetch.mockResolvedValueOnce({
        ok: true,
        json: () => Promise.resolve({ id: 1 })
      })
      
      await act(async () => {
        render(<OwnerCreateEditForm {...createModeProps} />)
      })
      
      const inputs = screen.getAllByDisplayValue('')
      const [firstNameInput, lastNameInput] = inputs
      const saveButton = screen.getByRole('button', { name: 'Add Owner' })
      
      await user.type(firstNameInput, 'John')
      await user.type(lastNameInput, 'Doe')
      await act(async () => {
        await user.click(saveButton)
      })
      
      expect(mockFetch).toHaveBeenCalledWith(
        '/api/owners',
        expect.objectContaining({
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: expect.stringContaining('"firstName":"John"')
        })
      )
    })
  })

  describe('Edit Mode', () => {
    const editModeProps = {
      hash: '#owners/1/edit',
      setHash: mockSetHash
    }

    test('fetches and displays existing owner data', async () => {
      const mockOwner = {
        id: 1,
        firstName: 'Jane',
        lastName: 'Smith',
        address: '456 Oak Ave',
        city: 'Springfield',
        telephone: '555-9876',
        pets: []
      }
      
      fetchOwnerById.mockResolvedValueOnce(mockOwner)
      
      await act(async () => {
        render(<OwnerCreateEditForm {...editModeProps} />)
      })
      
      await waitFor(() => {
        expect(fetchOwnerById).toHaveBeenCalledWith(1)
      })
      
      await waitFor(() => {
        expect(screen.getByDisplayValue('Jane')).toBeDefined()
        expect(screen.getByDisplayValue('Smith')).toBeDefined()
        expect(screen.getByDisplayValue('456 Oak Ave')).toBeDefined()
        expect(screen.getByDisplayValue('Springfield')).toBeDefined()
        expect(screen.getByDisplayValue('555-9876')).toBeDefined()
      })
    })

    test('handles fetch error gracefully', async () => {
      fetchOwnerById.mockRejectedValueOnce(new Error('Failed to fetch'))
      
      await act(async () => {
        render(<OwnerCreateEditForm {...editModeProps} />)
      })
      
      await waitFor(() => {
        expect(fetchOwnerById).toHaveBeenCalledWith(1)
      })
      
      // Form should still render with empty fields
      expect(screen.getByRole('button', { name: 'Edit Owner' })).toBeDefined()
    })

    test('submits updated owner data', async () => {
      const user = userEvent.setup()
      const mockFetch = global.fetch as jest.Mock
      const mockOwner = {
        id: 1,
        firstName: 'Jane',
        lastName: 'Smith',
        address: '456 Oak Ave',
        city: 'Springfield',
        telephone: '555-9876',
        pets: []
      }
      
      fetchOwnerById.mockResolvedValueOnce(mockOwner)
      mockFetch.mockResolvedValueOnce({
        ok: true,
        json: () => Promise.resolve({ id: 1 })
      })
      
      await act(async () => {
        render(<OwnerCreateEditForm {...editModeProps} />)
      })
      
      await waitFor(() => {
        expect(screen.getByDisplayValue('Jane')).toBeDefined()
      })
      
      const firstNameInput = screen.getByDisplayValue('Jane')
      const saveButton = screen.getByRole('button', { name: 'Edit Owner' })
      
      await user.clear(firstNameInput)
      await user.type(firstNameInput, 'Updated Jane')
      await act(async () => {
        await user.click(saveButton)
      })
      
      expect(mockFetch).toHaveBeenCalledWith(
        '/api/owners/1',
        expect.objectContaining({
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: expect.stringContaining('"firstName":"Updated Jane"')
        })
      )
    })
  })
})