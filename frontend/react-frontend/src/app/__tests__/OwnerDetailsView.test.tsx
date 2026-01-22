import React from 'react'
import { render, screen, waitFor, act } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import OwnerDetailsView from '../components/Owners/OwnerDetailsView'

// Mock the API module
jest.mock('../components/Api', () => ({
  fetchOwnerById: jest.fn(),
  fetchPetsForOwner: jest.fn(),
  fetchVisitsForPet: jest.fn()
}))

// Mock the Hash module
jest.mock('../components/Hash', () => ({
  flash: jest.fn(),
  replaceHash: jest.fn()
}))

// Mock the Translations component
jest.mock('../components/Translations', () => {
  return function T(key: string) {
    const translations: { [key: string]: string } = {
      'ownerInformation': 'ownerInformation',
      'name': 'Name',
      'address': 'address',
      'city': 'city',
      'telephone': 'telephone',
      'petsAndVisits': 'petsAndVisits',
      'editOwner': 'editOwner',
      'addNewPet': 'addNewPet',
      'editPet': 'editPet',
      'birthDate': 'Birth Date',
      'type': 'Type',
      'visitDate': 'Visit Date',
      'description': 'Description'
    }
    return translations[key] || key
  }
})

describe('OwnerDetailsView', () => {
  const mockSetHash = jest.fn()
  /* eslint-disable @typescript-eslint/no-require-imports */
  const { fetchOwnerById, fetchPetsForOwner, fetchVisitsForPet } = require('../components/Api')
  
  const defaultProps = {
    hash: '#owners/1',
    setHash: mockSetHash
  }

  const mockOwner = {
    id: 1,
    firstName: 'John',
    lastName: 'Doe',
    address: '123 Main St',
    city: 'Springfield',
    telephone: '555-0123',
    pets: []
  }

  const mockPets = [
    {
      id: 1,
      name: 'Fluffy',
      type: 'Cat',
      birthDate: '2020-01-01',
      visits: []
    },
    {
      id: 2,
      name: 'Rex',
      type: 'Dog', 
      birthDate: '2019-06-15',
      visits: []
    }
  ]

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

  beforeEach(() => {
    mockSetHash.mockClear()
    fetchOwnerById.mockClear()
    fetchPetsForOwner.mockClear()
    fetchVisitsForPet.mockClear()
  })

  test('renders owner details after loading', async () => {
    fetchOwnerById.mockResolvedValueOnce(mockOwner)
    fetchPetsForOwner.mockResolvedValueOnce([])
    
    await act(async () => {
      render(<OwnerDetailsView {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(screen.getByText(/John.*Doe/)).toBeDefined()
    })
  })

  test('renders owner information correctly', async () => {
    fetchOwnerById.mockResolvedValueOnce(mockOwner)
    fetchPetsForOwner.mockResolvedValueOnce([])
    
    await act(async () => {
      render(<OwnerDetailsView {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(fetchOwnerById).toHaveBeenCalledWith(1)
    })
    
    await waitFor(() => {
      expect(screen.getByText(/John.*Doe/)).toBeDefined()
      expect(screen.getByText('123 Main St')).toBeDefined()
      expect(screen.getByText('Springfield')).toBeDefined()
      expect(screen.getByText('555-0123')).toBeDefined()
    })
  })

  test('displays pets and their information', async () => {
    fetchOwnerById.mockResolvedValueOnce(mockOwner)
    fetchPetsForOwner.mockResolvedValueOnce(mockPets)
    fetchVisitsForPet.mockResolvedValue([])
    
    await act(async () => {
      render(<OwnerDetailsView {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(fetchPetsForOwner).toHaveBeenCalledWith(1)
    })
    
    await waitFor(() => {
      expect(screen.getByText('Fluffy')).toBeDefined()
      expect(screen.getByText('Rex')).toBeDefined()
      expect(screen.getByText('Cat')).toBeDefined()
      expect(screen.getByText('Dog')).toBeDefined()
    })
  })

  test('displays visits for each pet', async () => {
    fetchOwnerById.mockResolvedValueOnce(mockOwner)
    fetchPetsForOwner.mockResolvedValueOnce(mockPets)
    fetchVisitsForPet.mockResolvedValue(mockVisits)
    
    await act(async () => {
      render(<OwnerDetailsView {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(fetchVisitsForPet).toHaveBeenCalledWith(1, 1)
      expect(fetchVisitsForPet).toHaveBeenCalledWith(1, 2)
    })
    
    await waitFor(() => {
      expect(screen.getAllByText('Annual checkup')).toBeDefined()
      expect(screen.getAllByText('Vaccination')).toBeDefined()
    })
  })

  test('handles edit owner button click', async () => {
    const user = userEvent.setup()
    fetchOwnerById.mockResolvedValueOnce(mockOwner)
    fetchPetsForOwner.mockResolvedValueOnce([])
    
    await act(async () => {
      render(<OwnerDetailsView {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(screen.getByText(/John.*Doe/)).toBeDefined()
    })
    
    const editButton = screen.getByText('editOwner')
    await act(async () => {
      await user.click(editButton)
    })
    
    expect(mockSetHash).toHaveBeenCalledWith('#owners/1/edit')
  })

  test('handles add pet button click', async () => {
    const user = userEvent.setup()
    fetchOwnerById.mockResolvedValueOnce(mockOwner)
    fetchPetsForOwner.mockResolvedValueOnce([])
    
    await act(async () => {
      render(<OwnerDetailsView {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(screen.getByText(/John.*Doe/)).toBeDefined()
    })
    
    const addPetButton = screen.getByText('addNewPet')
    await act(async () => {
      await user.click(addPetButton)
    })
    
    expect(mockSetHash).toHaveBeenCalledWith('#owners/1/pets/new')
  })

  test('handles error state gracefully', async () => {
    fetchOwnerById.mockRejectedValueOnce(new Error('Failed to fetch owner'))
    
    await act(async () => {
      render(<OwnerDetailsView {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(fetchOwnerById).toHaveBeenCalledWith(1)
    })
    
    await waitFor(() => {
      expect(screen.getByText(/Error loading owner details.*Failed to fetch owner/)).toBeDefined()
    })
  })

  test('extracts owner ID from hash correctly', async () => {
    const hashProps = {
      hash: '#owners/42',
      setHash: mockSetHash
    }
    
    fetchOwnerById.mockResolvedValueOnce(mockOwner)
    fetchPetsForOwner.mockResolvedValueOnce([])
    
    await act(async () => {
      render(<OwnerDetailsView {...hashProps} />)
    })
    
    expect(fetchOwnerById).toHaveBeenCalledWith(42)
  })
})