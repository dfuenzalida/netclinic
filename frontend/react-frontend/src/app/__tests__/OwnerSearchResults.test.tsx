import React from 'react'
import { render, screen, waitFor, act } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import OwnerSearchResults from '../components/Owners/OwnerSearchResults'

// Mock the API module
jest.mock('../components/Api', () => ({
  fetchPetsForOwner: jest.fn()
}))

// Mock the Hash module
jest.mock('../components/Hash', () => ({
  replaceHash: jest.fn()
}))

// Mock the Translations component
jest.mock('../components/Translations', () => {
  return function T(key: string) {
    const translations: { [key: string]: string } = {
      'owners': 'Owners',
      'findOwners': 'Find Owners',
      'addOwner': 'Add Owner',
      'firstName': 'First Name',
      'lastName': 'Last Name',
      'address': 'Address',
      'city': 'City',
      'telephone': 'Telephone',
      'pets': 'Pets',
      'loading': 'Loading...',
      'noResults': 'No owners found',
      'showingResultsFor': 'Showing results for'
    }
    return translations[key] || key
  }
})

// Mock Pagination component
jest.mock('../components/Pagination', () => {
  return function Pagination({ currentPage, totalPages, setCurrentPage }: any) {
    return (
      <div data-testid="pagination">
        <button 
          onClick={() => setCurrentPage(currentPage - 1)} 
          disabled={currentPage <= 1}
        >
          Previous
        </button>
        <span>{currentPage} of {totalPages}</span>
        <button 
          onClick={() => setCurrentPage(currentPage + 1)}
          disabled={currentPage >= totalPages}
        >
          Next
        </button>
      </div>
    )
  }
})

// Mock fetch
global.fetch = jest.fn()

describe('OwnerSearchResults', () => {
  const mockSetHash = jest.fn()
  const { fetchPetsForOwner } = require('../components/Api')
  const { replaceHash } = require('../components/Hash')
  
  const defaultProps = {
    hash: '#owners/lastName/Smith',
    setHash: mockSetHash
  }

  const mockOwners = [
    {
      id: 1,
      firstName: 'John',
      lastName: 'Smith',
      address: '123 Main St',
      city: 'Springfield',
      telephone: '555-0123',
      pets: []
    },
    {
      id: 2,
      firstName: 'Jane',
      lastName: 'Smith',
      address: '456 Oak Ave',
      city: 'Springfield',
      telephone: '555-9876',
      pets: []
    }
  ]

  const mockApiResponse = {
    ownerList: mockOwners,
    totalPages: 1,
    currentPage: 1
  }

  let consoleLogSpy: jest.SpyInstance

  beforeEach(() => {
    mockSetHash.mockClear()
    fetchPetsForOwner.mockClear()
    replaceHash.mockClear()
    ;(global.fetch as jest.Mock).mockClear()
    
    // Suppress console logs by default to reduce test noise
    consoleLogSpy = jest.spyOn(console, 'log').mockImplementation(() => {})
  })

  afterEach(() => {
    consoleLogSpy.mockRestore()
  })

  test('displays search results after loading', async () => {
    ;(global.fetch as jest.Mock).mockResolvedValueOnce({
      ok: true,
      json: () => Promise.resolve(mockApiResponse)
    })
    fetchPetsForOwner.mockResolvedValue([])
    
    await act(async () => {
      render(<OwnerSearchResults {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(screen.getByText(/John\s+Smith/)).toBeDefined()
      expect(screen.getByText(/Jane\s+Smith/)).toBeDefined()
    })
  })

  test('displays search results correctly', async () => {
    ;(global.fetch as jest.Mock).mockResolvedValueOnce({
      ok: true,
      json: () => Promise.resolve(mockApiResponse)
    })
    fetchPetsForOwner.mockResolvedValue([])
    
    await act(async () => {
      render(<OwnerSearchResults {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(screen.getByText(/John\s+Smith/)).toBeDefined()
      expect(screen.getByText(/Jane\s+Smith/)).toBeDefined()
      expect(screen.getByText('123 Main St')).toBeDefined()
      expect(screen.getByText('456 Oak Ave')).toBeDefined()
    })
  })

  test('extracts lastName from hash correctly', async () => {
    ;(global.fetch as jest.Mock).mockResolvedValueOnce({
      ok: true,
      json: () => Promise.resolve(mockApiResponse)
    })
    
    await act(async () => {
      render(<OwnerSearchResults {...defaultProps} />)
    })
    
    expect(global.fetch).toHaveBeenCalledWith('/api/owners?lastName=Smith&page=1')
  })

  test('handles hash with query parameters', async () => {
    const propsWithQuery = {
      hash: '#owners/lastName/Johnson?flash=Success',
      setHash: mockSetHash
    }
    
    ;(global.fetch as jest.Mock).mockResolvedValueOnce({
      ok: true,
      json: () => Promise.resolve(mockApiResponse)
    })
    
    await act(async () => {
      render(<OwnerSearchResults {...propsWithQuery} />)
    })
    
    expect(global.fetch).toHaveBeenCalledWith('/api/owners?lastName=Johnson&page=1')
  })

  test('redirects to owner details when single result', async () => {
    const singleOwnerResponse = {
      ownerList: [mockOwners[0]],
      totalPages: 1,
      currentPage: 1
    }
    
    ;(global.fetch as jest.Mock).mockResolvedValueOnce({
      ok: true,
      json: () => Promise.resolve(singleOwnerResponse)
    })
    
    await act(async () => {
      render(<OwnerSearchResults {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(replaceHash).toHaveBeenCalledWith('#owners/1')
    })
  })

  test('redirects to search when no results', async () => {
    const noResultsResponse = {
      ownerList: [],
      totalPages: 0,
      currentPage: 1
    }
    
    ;(global.fetch as jest.Mock).mockResolvedValueOnce({
      ok: true,
      json: () => Promise.resolve(noResultsResponse)
    })
    
    await act(async () => {
      render(<OwnerSearchResults {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(mockSetHash).toHaveBeenCalledWith('#owners/search')
    })
  })

  test('handles owner click navigation', async () => {
    const user = userEvent.setup()
    ;(global.fetch as jest.Mock).mockResolvedValueOnce({
      ok: true,
      json: () => Promise.resolve(mockApiResponse)
    })
    fetchPetsForOwner.mockResolvedValue([])
    
    await act(async () => {
      render(<OwnerSearchResults {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(screen.getByText(/John\s+Smith/)).toBeDefined()
    })
    
    const johnLink = screen.getByText(/John\s+Smith/)
    await act(async () => {
      await user.click(johnLink)
    })
    expect(mockSetHash).toHaveBeenCalledWith('#owners/1')
  })

  test('displays pets for each owner', async () => {
    const mockPets = [
      { id: 1, name: 'Fluffy', type: 'Cat', birthDate: '2020-01-01', visits: [] }
    ]
    
    ;(global.fetch as jest.Mock).mockResolvedValueOnce({
      ok: true,
      json: () => Promise.resolve(mockApiResponse)
    })
    fetchPetsForOwner.mockResolvedValue(mockPets)
    
    await act(async () => {
      render(<OwnerSearchResults {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(fetchPetsForOwner).toHaveBeenCalledWith(1)
      expect(fetchPetsForOwner).toHaveBeenCalledWith(2)
    })
    
    await waitFor(() => {
      expect(screen.getAllByText('Fluffy')).toHaveLength(2)
    })
  })

  test('handles API error gracefully', async () => {
    ;(global.fetch as jest.Mock).mockRejectedValueOnce(new Error('API Error'))
    
    await act(async () => {
      render(<OwnerSearchResults {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(screen.getByText(/Error loading owners.*API Error/)).toBeDefined()
    })
  })

  test('handles pagination correctly', async () => {
    const multiPageResponse = {
      ownerList: mockOwners,
      totalPages: 3,
      currentPage: 1
    }
    
    ;(global.fetch as jest.Mock).mockResolvedValueOnce({
      ok: true,
      json: () => Promise.resolve(multiPageResponse)
    })
    fetchPetsForOwner.mockResolvedValue([])
    
    await act(async () => {
      render(<OwnerSearchResults {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(screen.getByTestId('pagination')).toBeDefined()
    })
    
    // Verify initial fetch was made
    expect(global.fetch).toHaveBeenCalledWith('/api/owners?lastName=Smith&page=1')
    
    // Test that pagination component is rendered with correct props
    expect(screen.getByText('1 of 3')).toBeDefined()
  })

  test('handles HTTP error response', async () => {
    ;(global.fetch as jest.Mock).mockResolvedValueOnce({
      ok: false,
      status: 500
    })
    
    await act(async () => {
      render(<OwnerSearchResults {...defaultProps} />)
    })
    
    await waitFor(() => {
      expect(screen.getByText(/Error loading owners.*HTTP error! status: 500/)).toBeDefined()
    })
  })
})