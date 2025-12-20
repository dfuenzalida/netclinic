import React from 'react'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import OwnerSearchForm from '../components/Owners/OwnerSearchForm'

// Mock the Translations component
jest.mock('../components/Translations', () => {
  return function T(key: string) {
    const translations: { [key: string]: string } = {
      'findOwners': 'Find Owners',
      'lastName': 'Last Name',
      'findOwner': 'Find Owner',
      'addOwner': 'Add Owner'
    }
    return translations[key] || key
  }
})

describe('OwnerSearchForm', () => {
  const mockSetHash = jest.fn()
  const defaultProps = {
    hash: '#owners/search',
    setHash: mockSetHash
  }
  
  beforeEach(() => {
    mockSetHash.mockClear()
  })

  test('renders search form correctly', () => {
    render(<OwnerSearchForm {...defaultProps} />)
    
    expect(screen.getByText('Find Owners')).toBeInTheDocument()
    expect(screen.getByDisplayValue('')).toBeInTheDocument() // The input field
    expect(screen.getByRole('button', { name: 'Find Owner' })).toBeInTheDocument()
    expect(screen.getByText('Add Owner')).toBeInTheDocument()
  })

  test('allows user to enter last name', async () => {
    const user = userEvent.setup()
    render(<OwnerSearchForm {...defaultProps} />)
    
    const lastNameInput = screen.getByDisplayValue('') // Get the input by its value
    await user.type(lastNameInput, 'Smith')
    
    expect(lastNameInput).toHaveValue('Smith')
  })

  test('triggers search when Find Owner button is clicked', async () => {
    const user = userEvent.setup()
    render(<OwnerSearchForm {...defaultProps} />)
    
    const lastNameInput = screen.getByDisplayValue('')
    const searchButton = screen.getByRole('button', { name: 'Find Owner' })
    
    await user.type(lastNameInput, 'Johnson')
    await user.click(searchButton)
    
    expect(mockSetHash).toHaveBeenCalledWith('#owners/lastName/Johnson')
  })

  test('triggers search when Enter key is pressed in input field', async () => {
    const user = userEvent.setup()
    render(<OwnerSearchForm {...defaultProps} />)
    
    const lastNameInput = screen.getByDisplayValue('')
    await user.type(lastNameInput, 'Brown{enter}')
    
    expect(mockSetHash).toHaveBeenCalledWith('#owners/lastName/Brown')
  })

  test('navigates to add owner form when Add Owner is clicked', async () => {
    const user = userEvent.setup()
    render(<OwnerSearchForm {...defaultProps} />)
    
    const addOwnerLink = screen.getByText('Add Owner')
    await user.click(addOwnerLink)
    
    expect(mockSetHash).toHaveBeenCalledWith('#owners/new')
  })

  test('handles empty search', async () => {
    const user = userEvent.setup()
    render(<OwnerSearchForm {...defaultProps} />)
    
    const searchButton = screen.getByRole('button', { name: 'Find Owner' })
    await user.click(searchButton)
    
    expect(mockSetHash).toHaveBeenCalledWith('#owners/lastName/')
  })
})