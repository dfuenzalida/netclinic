import React from 'react'
import { render, screen, waitFor, act } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import CreateVisitForm from '../components/Pets/CreateVisitForm'

// Mock the API module
jest.mock('../components/Api', () => ({
  fetchOwnerById: jest.fn(),
  fetchPetById: jest.fn(),
  fetchPetVisitsById: jest.fn()
}))

// Mock the Translations component
jest.mock('../components/Translations', () => {
  return function T(key: string) {
    const translations: { [key: string]: string } = {
      'visit': 'Visit',
      'owner': 'Owner',
      'pet': 'Pet',
      'date': 'date',
      'description': 'Description',
      'addVisit': 'addVisit',
      'cancel': 'Cancel',
      'newVisit': 'New Visit',
      'previousVisits': 'Previous Visits',
      'requiredField': 'This field is required',
      'invalidDate': 'Invalid date format',
      'name': 'Name',
      'type': 'Type',
      'birthDate': 'Birth Date'
    }
    return translations[key] || key
  }
})

// Mock fetch
global.fetch = jest.fn()

describe('CreateVisitForm', () => {
  const mockSetHash = jest.fn()
  /* eslint-disable @typescript-eslint/no-require-imports */
  const { fetchOwnerById, fetchPetById, fetchPetVisitsById } = require('../components/Api')
  
  const mockOwner = {
    id: 1,
    firstName: 'John',
    lastName: 'Doe',
    address: '123 Main St',
    city: 'Springfield',
    telephone: '555-0123',
    pets: []
  }

  const mockPet = {
    id: 1,
    name: 'Fluffy',
    type: 'Cat',
    birthDate: '2020-01-01',
    visits: []
  }

  const mockVisits = [
    {
      id: 1,
      visitDate: '2023-01-15',
      description: 'Annual checkup'
    },
    {
      id: 2,
      visitDate: '2023-06-20',
      description: 'Vaccination'
    }
  ]

  const defaultProps = {
    hash: '#owners/1/pets/1/visits/new',
    setHash: mockSetHash
  }

  let consoleWarnSpy: jest.SpyInstance
  let consoleErrorSpy: jest.SpyInstance

  beforeEach(() => {
    mockSetHash.mockClear()
    fetchOwnerById.mockClear()
    fetchPetById.mockClear()
    fetchPetVisitsById.mockClear()
    ;(global.fetch as jest.Mock).mockClear()
    
    // Suppress console warnings and errors by default to reduce test noise
    consoleWarnSpy = jest.spyOn(console, 'warn').mockImplementation(() => {})
    consoleErrorSpy = jest.spyOn(console, 'error').mockImplementation(() => {})
    
    // Default successful mocks
    fetchOwnerById.mockResolvedValue(mockOwner)
    fetchPetById.mockResolvedValue(mockPet)
    fetchPetVisitsById.mockResolvedValue(mockVisits)
  })

  afterEach(() => {
    consoleWarnSpy.mockRestore()
    consoleErrorSpy.mockRestore()
  })

  test('renders create visit form correctly', async () => {
    await act(async () => {
      render(<CreateVisitForm {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(screen.getByText(/John/)).toBeDefined()
      expect(screen.getByText(/Doe/)).toBeDefined()
      expect(screen.getByText('Fluffy')).toBeDefined()
    })
    
    expect(screen.getByLabelText('date')).toBeDefined()
    expect(screen.getByLabelText('Description')).toBeDefined()
    expect(screen.getByRole('button', { name: 'addVisit' })).toBeDefined()
  })

  test('fetches owner information', async () => {
    await act(async () => {
      render(<CreateVisitForm {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(fetchOwnerById).toHaveBeenCalledWith(1)
      expect(screen.getByText(/John/)).toBeDefined()
      expect(screen.getByText(/Doe/)).toBeDefined()
    })
  })

  test('fetches pet information', async () => {
    await act(async () => {
      render(<CreateVisitForm {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(fetchPetById).toHaveBeenCalledWith(1, 1)
      expect(screen.getByText('Fluffy')).toBeDefined()
    })
  })

  test('fetches and displays previous visits', async () => {
    await act(async () => {
      render(<CreateVisitForm {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(fetchPetVisitsById).toHaveBeenCalledWith(1, 1)
    })
    
    await waitFor(() => {
      expect(screen.getByText('Annual checkup')).toBeDefined()
      expect(screen.getByText('Vaccination')).toBeDefined()
      expect(screen.getByText('2023-01-15')).toBeDefined()
      expect(screen.getByText('2023-06-20')).toBeDefined()
    })
  })

  test('allows user to enter visit information', async () => {
    const user = userEvent.setup()
    await act(async () => {
      render(<CreateVisitForm {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(screen.getByText('Fluffy')).toBeDefined()
    })
    
    const inputs = screen.getAllByDisplayValue('')
    const [visitDateInput, descriptionInput] = inputs
    
    await user.type(visitDateInput, '2024-01-15')
    await user.type(descriptionInput, 'Routine checkup')
    
    // Verify the values are set (using a different approach)
    expect(visitDateInput.getAttribute('value')).toBe('2024-01-15')
    expect(descriptionInput.getAttribute('value')).toBe('Routine checkup')
  })

  test('submits form with correct data', async () => {
    const user = userEvent.setup()
    const mockFetch = global.fetch as jest.Mock
    mockFetch.mockResolvedValueOnce({
      ok: true,
      json: () => Promise.resolve({ id: 3 })
    })
    
    await act(async () => {
      render(<CreateVisitForm {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(screen.getByRole('button', { name: 'addVisit' })).toBeDefined()
    })
    
    const inputs = screen.getAllByDisplayValue('')
    const [visitDateInput, descriptionInput] = inputs
    const saveButton = screen.getByRole('button', { name: 'addVisit' })
    
    await user.type(visitDateInput, '2024-01-15')
    await user.type(descriptionInput, 'Routine checkup')
    await act(async () => {
      await user.click(saveButton)
    })
    
    expect(mockFetch).toHaveBeenCalledWith(
      '/api/owners/1/pets/1/visits',
      expect.objectContaining({
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: expect.stringContaining('"description":"Routine checkup"')
      })
    )
  })

  test('handles owner fetch error gracefully', async () => {
    fetchOwnerById.mockRejectedValueOnce(new Error('Failed to fetch owner'))
    
    await act(async () => {
      render(<CreateVisitForm {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(fetchOwnerById).toHaveBeenCalledWith(1)
    })
    
    // Form should still render
    expect(screen.getByRole('button', { name: 'addVisit' })).toBeDefined()
  })

  test('handles pet fetch error gracefully', async () => {
    fetchPetById.mockRejectedValueOnce(new Error('Failed to fetch pet'))
    
    await act(async () => {
      render(<CreateVisitForm {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(fetchPetById).toHaveBeenCalledWith(1, 1)
    })
    
    // Form should still render
    expect(screen.getByRole('button', { name: 'addVisit' })).toBeDefined()
  })

  test('handles visits fetch error gracefully', async () => {
    fetchPetVisitsById.mockRejectedValueOnce(new Error('Failed to fetch visits'))
    
    await act(async () => {
      render(<CreateVisitForm {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(fetchPetVisitsById).toHaveBeenCalledWith(1, 1)
    })
    
    // Form should still render
    expect(screen.getByRole('button', { name: 'addVisit' })).toBeDefined()
  })

  test('extracts owner and pet IDs from hash correctly', async () => {
    const props = {
      hash: '#owners/42/pets/99/visits/new',
      setHash: mockSetHash
    }
    
    await act(async () => {
      render(<CreateVisitForm {...props} />)
    })
    
    expect(fetchOwnerById).toHaveBeenCalledWith(42)
    expect(fetchPetById).toHaveBeenCalledWith(42, 99)
    expect(fetchPetVisitsById).toHaveBeenCalledWith(42, 99)
  })

  test('validates form submission', async () => {
    const user = userEvent.setup()
    const mockFetch = global.fetch as jest.Mock
    mockFetch.mockResolvedValueOnce({
      ok: false,
      json: () => Promise.resolve({
        errors: { visitDate: 'Visit date is required' }
      })
    })
    
    await act(async () => {
      render(<CreateVisitForm {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(screen.getByRole('button', { name: 'addVisit' })).toBeDefined()
    })
    
    const saveButton = screen.getByRole('button', { name: 'addVisit' })
    await act(async () => {
      await user.click(saveButton)
    })
    
    expect(mockFetch).toHaveBeenCalled()
  })

  test('shows previous visits section when visits exist', async () => {
    await act(async () => {
      render(<CreateVisitForm {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(screen.getByText('Previous Visits')).toBeDefined()
      expect(screen.getByText('Annual checkup')).toBeDefined()
      expect(screen.getByText('Vaccination')).toBeDefined()
    })
  })

  test('handles empty visits list', async () => {
    fetchPetVisitsById.mockResolvedValueOnce([])
    
    await act(async () => {
      render(<CreateVisitForm {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(fetchPetVisitsById).toHaveBeenCalledWith(1, 1)
    })
    
    // Form should still render without previous visits
    expect(screen.getByRole('button', { name: 'addVisit' })).toBeDefined()
  })

  test('navigates back to owner details on successful save', async () => {
    const user = userEvent.setup()
    const mockFetch = global.fetch as jest.Mock
    mockFetch.mockResolvedValueOnce({
      ok: true,
      json: () => Promise.resolve({ id: 3 })
    })
    
    await act(async () => {
      render(<CreateVisitForm {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(screen.getByRole('button', { name: 'addVisit' })).toBeDefined()
    })
    
    const inputs = screen.getAllByDisplayValue('')
    const [visitDateInput, descriptionInput] = inputs
    const saveButton = screen.getByRole('button', { name: 'addVisit' })
    
    await user.type(visitDateInput, '2024-01-15')
    await user.type(descriptionInput, 'Routine checkup')
    await act(async () => {
      await user.click(saveButton)
    })
    
    await waitFor(() => {
      expect(mockSetHash).toHaveBeenCalledWith('#owners/1/pets/1?flash=Visit Created')
    })
  })
})