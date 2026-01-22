import React from 'react'
import { render, screen, waitFor, act } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import PetCreateEditForm from '../components/Pets/PetCreateEditForm'

// Mock the API module
jest.mock('../components/Api', () => ({
  fetchOwnerById: jest.fn(),
  fetchPetById: jest.fn(),
  fetchPetTypes: jest.fn()
}))

// Mock the Translations component
jest.mock('../components/Translations', () => {
  return function T(key: string) {
    const translations: { [key: string]: string } = {
      'pet': 'Pet',
      'owner': 'Owner',
      'name': 'Name',
      'birthDate': 'Birth Date',
      'type': 'Type',
      'save': 'Save',
      'cancel': 'Cancel',
      'newPet': 'New Pet',
      'editPet': 'Edit Pet',
      'addPet': 'Add Pet',
      'addNewPet': 'addNewPet',
      'requiredField': 'This field is required',
      'invalidDate': 'Invalid date format',
      'selectType': 'Select a type'
    }
    return translations[key] || key
  }
})

// Mock fetch
global.fetch = jest.fn()

describe('PetCreateEditForm', () => {
  const mockSetHash = jest.fn()
  /* eslint-disable @typescript-eslint/no-require-imports */
  const { fetchOwnerById, fetchPetById, fetchPetTypes } = require('../components/Api')
  
  const mockOwner = {
    id: 1,
    firstName: 'John',
    lastName: 'Doe',
    address: '123 Main St',
    city: 'Springfield',
    telephone: '555-0123',
    pets: []
  }

  const mockPetTypes = [
    { id: 1, name: 'Cat' },
    { id: 2, name: 'Dog' },
    { id: 3, name: 'Bird' }
  ]

  const mockPet = {
    id: 1,
    name: 'Fluffy',
    type: 'Cat',
    birthDate: '2020-01-01',
    visits: []
  }

  beforeEach(() => {
    mockSetHash.mockClear()
    fetchOwnerById.mockClear()
    fetchPetById.mockClear()
    fetchPetTypes.mockClear()
    ;(global.fetch as jest.Mock).mockClear()
    
    // Default successful mocks
    fetchOwnerById.mockResolvedValue(mockOwner)
    fetchPetTypes.mockResolvedValue(mockPetTypes)
  })

  describe('Create Mode', () => {
    const createModeProps = {
      hash: '#owners/1/pets/new',
      setHash: mockSetHash
    }

    test('renders create form correctly', async () => {
      await act(async () => {
        render(<PetCreateEditForm {...createModeProps} />)
      })
      
      await waitFor(() => {
        expect(screen.getByText(/John\s+Doe/)).toBeDefined()
      })
      
      expect(screen.getByText('Name')).toBeDefined()
      expect(screen.getByText('Birth Date')).toBeDefined()
      expect(screen.getByText('Type')).toBeDefined()
      expect(screen.getByRole('button', { name: 'addNewPet' })).toBeDefined()
    })

    test('fetches and displays owner information', async () => {
      await act(async () => {
        render(<PetCreateEditForm {...createModeProps} />)
      })
      
      await waitFor(() => {
        expect(fetchOwnerById).toHaveBeenCalledWith(1)
        expect(screen.getByText(/John/)).toBeDefined()
        expect(screen.getByText(/Doe/)).toBeDefined()
      })
    })

    test('loads pet types in dropdown', async () => {
      await act(async () => {
        render(<PetCreateEditForm {...createModeProps} />)
      })
      
      await waitFor(() => {
        expect(fetchPetTypes).toHaveBeenCalled()
      })
      
      await waitFor(() => {
        expect(screen.getByText('Cat')).toBeDefined()
        expect(screen.getByText('Dog')).toBeDefined()
        expect(screen.getByText('Bird')).toBeDefined()
      })
    })

    test('allows user to enter pet information', async () => {
      const user = userEvent.setup()
      await act(async () => {
        render(<PetCreateEditForm {...createModeProps} />)
      })
      
      await waitFor(() => {
        expect(screen.getByText(/John/)).toBeDefined()
      })
      
      const nameInput = screen.getByLabelText('Name')
      const birthDateInput = screen.getByLabelText('Birth Date')
      
      await user.type(nameInput, 'Rex')
      await user.type(birthDateInput, '2022-06-15')
      
      // Verify the values are set
      expect(nameInput).toHaveValue('Rex')
      expect(birthDateInput).toHaveValue('2022-06-15')
    })

    test('submits form with correct data', async () => {
      const user = userEvent.setup()
      const mockFetch = global.fetch as jest.Mock
      mockFetch.mockResolvedValueOnce({
        ok: true,
        json: () => Promise.resolve({ id: 2 })
      })
      
      await act(async () => {
        render(<PetCreateEditForm {...createModeProps} />)
      })
      
      await waitFor(() => {
        expect(screen.getByRole('button', { name: 'addNewPet' })).toBeDefined()
      })
      
      const nameInput = screen.getByLabelText('Name')
      const saveButton = screen.getByRole('button', { name: 'addNewPet' })
      
      await user.type(nameInput, 'Rex')
      await act(async () => {
        await user.click(saveButton)
      })
      
      expect(mockFetch).toHaveBeenCalledWith(
        '/api/owners/1/pets',
        expect.objectContaining({
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: expect.stringContaining('"name":"Rex"')
        })
      )
    })
  })

  describe('Edit Mode', () => {
    const editModeProps = {
      hash: '#owners/1/pets/1/edit',
      setHash: mockSetHash
    }

    test('fetches and displays existing pet data', async () => {
      fetchPetById.mockResolvedValueOnce(mockPet)
      
      await act(async () => {
        render(<PetCreateEditForm {...editModeProps} />)
      })
      
      await waitFor(() => {
        expect(fetchPetById).toHaveBeenCalledWith(1, 1)
      })
      
      await waitFor(() => {
        expect(screen.getByDisplayValue('Fluffy')).toBeDefined()
        expect(screen.getByDisplayValue('2020-01-01')).toBeDefined()
      })
    })

    test('submits updated pet data', async () => {
      const user = userEvent.setup()
      const mockFetch = global.fetch as jest.Mock
      
      fetchPetById.mockResolvedValueOnce(mockPet)
      mockFetch.mockResolvedValueOnce({
        ok: true,
        json: () => Promise.resolve({ id: 1 })
      })
      
      await act(async () => {
        render(<PetCreateEditForm {...editModeProps} />)
      })
      
      await waitFor(() => {
        expect(screen.getByDisplayValue('Fluffy')).toBeDefined()
      })
      
      const nameInput = screen.getByDisplayValue('Fluffy')
      const saveButton = screen.getByText('Edit Pet')
      
      await user.clear(nameInput)
      await user.type(nameInput, 'Updated Fluffy')
      await act(async () => {
        await user.click(saveButton)
      })
      
      expect(mockFetch).toHaveBeenCalledWith(
        '/api/owners/1/pets/1',
        expect.objectContaining({
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: expect.stringContaining('"name":"Updated Fluffy"')
        })
      )
    })

    test('handles pet fetch error gracefully', async () => {
      fetchPetById.mockRejectedValueOnce(new Error('Failed to fetch pet'))
      
      // Suppress console.warn for this test since we're intentionally triggering an error
      const consoleSpy = jest.spyOn(console, 'warn').mockImplementation(() => {})
      
      await act(async () => {
        render(<PetCreateEditForm {...editModeProps} />)
      })
      
      await waitFor(() => {
        expect(fetchPetById).toHaveBeenCalledWith(1, 1)
      })
      
      // Form should still render
      expect(screen.getByText('Edit Pet')).toBeDefined()
      
      consoleSpy.mockRestore()
    })
  })

  test('handles owner fetch error gracefully', async () => {
    fetchOwnerById.mockRejectedValueOnce(new Error('Failed to fetch owner'))
    
    // Suppress console.warn for this test since we're intentionally triggering an error
    const consoleSpy = jest.spyOn(console, 'warn').mockImplementation(() => {})
    
    const props = {
      hash: '#owners/1/pets/new',
      setHash: mockSetHash
    }
    
    await act(async () => {
      render(<PetCreateEditForm {...props} />)
    })
    
    await waitFor(() => {
      expect(fetchOwnerById).toHaveBeenCalledWith(1)
    })
    
    // Form should still render even if owner fetch fails
    expect(screen.getByRole('button', { name: 'addNewPet' })).toBeDefined()
    
    consoleSpy.mockRestore()
  })

  test('handles pet types fetch error gracefully', async () => {
    fetchPetTypes.mockRejectedValueOnce(new Error('Failed to fetch pet types'))
    
    // Suppress console.warn for this test since we're intentionally triggering an error
    const consoleSpy = jest.spyOn(console, 'warn').mockImplementation(() => {})
    
    const props = {
      hash: '#owners/1/pets/new',
      setHash: mockSetHash
    }
    
    await act(async () => {
      render(<PetCreateEditForm {...props} />)
    })
    
    await waitFor(() => {
      expect(fetchPetTypes).toHaveBeenCalled()
    })
    
    // Form should still render
    expect(screen.getByRole('button', { name: 'addNewPet' })).toBeDefined()
    
    consoleSpy.mockRestore()
  })

  test('validates form submission', async () => {
    const user = userEvent.setup()
    const mockFetch = global.fetch as jest.Mock
    mockFetch.mockResolvedValueOnce({
      ok: false,
      json: () => Promise.resolve({
        errors: { name: 'Name is required' }
      })
    })
    
    // Suppress console.error for this test since we're intentionally triggering an error
    const consoleSpy = jest.spyOn(console, 'error').mockImplementation(() => {})
    
    const props = {
      hash: '#owners/1/pets/new',
      setHash: mockSetHash
    }
    
    await act(async () => {
      render(<PetCreateEditForm {...props} />)
    })
    
    await waitFor(() => {
      expect(screen.getByRole('button', { name: 'addNewPet' })).toBeDefined()
    })
    
    const saveButton = screen.getByRole('button', { name: 'addNewPet' })
    await act(async () => {
      await user.click(saveButton)
    })
    
    expect(mockFetch).toHaveBeenCalled()
    
    consoleSpy.mockRestore()
  })

  test('extracts owner and pet IDs from hash correctly', async () => {
    const props = {
      hash: '#owners/42/pets/99/edit',
      setHash: mockSetHash
    }
    
    fetchPetById.mockResolvedValueOnce(mockPet)
    
    await act(async () => {
      render(<PetCreateEditForm {...props} />)
    })
    
    expect(fetchOwnerById).toHaveBeenCalledWith(42)
    expect(fetchPetById).toHaveBeenCalledWith(42, 99)
  })
})