# Testing Setup for NetClinic Frontend

## Overview

This project now includes a complete Jest and React Testing Library setup for testing your React/Next.js veterinary clinic management application.

## Test Environment

- **Jest**: JavaScript testing framework
- **React Testing Library**: Testing utilities for React components
- **Jest DOM**: Custom Jest matchers for DOM node assertions
- **User Event**: Advanced simulation of user interactions

## Running Tests

```bash
# Run all tests once
npm test

# Run tests in watch mode (re-runs when files change)
npm run test:watch

# Run tests with coverage report
npm run test:coverage

# Run specific test files
npm test -- OwnerSearchForm

# Run tests matching a pattern
npm test -- --testNamePattern="renders correctly"
```

## Test Structure

### Test Files Location
All test files are located in `src/app/__tests__/` with the following structure:

```
src/app/__tests__/
├── setup.test.tsx           # Basic Jest setup verification
├── OwnerSearchForm.test.tsx # Owner search form component tests
├── NavBar.test.tsx          # Navigation component tests  
├── Hash.test.tsx           # Custom hash routing hook tests
├── App.test.tsx            # Main app component tests
└── test-utils.tsx          # Shared testing utilities
```

### Test Types Implemented

1. **Component Tests** - Testing individual React components
2. **User Interaction Tests** - Testing user events like clicking and typing
3. **Hook Tests** - Testing custom React hooks
4. **Integration Tests** - Testing component interactions

## Example Tests

### Component Rendering Test
```tsx
test('renders search form correctly', () => {
  render(<OwnerSearchForm {...defaultProps} />)
  
  expect(screen.getByText('Find Owners')).toBeDefined()
  expect(screen.getByRole('button', { name: 'Find Owner' })).toBeDefined()
})
```

### User Interaction Test
```tsx
test('triggers search when button is clicked', async () => {
  const user = userEvent.setup()
  const mockSetHash = jest.fn()
  
  render(<OwnerSearchForm hash="#search" setHash={mockSetHash} />)
  
  const input = screen.getByDisplayValue('')
  const button = screen.getByRole('button', { name: 'Find Owner' })
  
  await user.type(input, 'Johnson')
  await user.click(button)
  
  expect(mockSetHash).toHaveBeenCalledWith('#owners/lastName/Johnson')
})
```

### Custom Hook Test
```tsx
test('updates hash when updateHash is called', () => {
  const { result } = renderHook(() => useHash())
  
  act(() => {
    const [, updateHash] = result.current
    updateHash('#new-hash')
  })
  
  expect(window.location.hash).toBe('#new-hash')
})
```

## Testing Utilities

### Custom Mock Creators
The `test-utils.tsx` file provides helpful utilities:

```tsx
// Create mock props for components
const props = createMockHashProps('#owners/search')

// Create mock data objects
const owner = createMockOwner({ firstName: 'John', lastName: 'Doe' })
const pet = createMockPet({ name: 'Fluffy', type: 'Cat' })
const vet = createMockVet({ firstName: 'Dr. Jane' })
```

## Test Configuration

### Jest Configuration (`jest.config.js`)
- Uses Next.js Jest configuration
- JSDOM test environment for DOM testing
- Automatic test discovery
- Coverage collection setup

### Setup File (`jest.setup.js`)
- Imports Jest DOM matchers
- Configures global test utilities
- Mocks for ResizeObserver and other browser APIs

## Best Practices Implemented

1. **Descriptive Test Names** - Tests clearly describe what they're testing
2. **Proper Mocking** - Components and dependencies are properly mocked
3. **User-Centric Testing** - Tests focus on user interactions, not implementation details
4. **Cleanup** - Tests clean up after themselves using `beforeEach`
5. **Type Safety** - Full TypeScript support in tests

## Coverage

Run `npm run test:coverage` to see test coverage report. The setup is configured to collect coverage from:
- All TypeScript/JavaScript files in `src/`
- Excludes type definitions and story files

## Next Steps

### Recommended Additional Tests

1. **API Integration Tests** - Test components that fetch data
2. **Error Boundary Tests** - Test error handling
3. **Accessibility Tests** - Use `jest-axe` for a11y testing
4. **Visual Regression Tests** - Add Storybook for component documentation

### Example: Testing API Components
```tsx
// For components that fetch data
test('displays loading state while fetching vets', async () => {
  const mockFetch = jest.fn().mockResolvedValue({
    ok: true,
    json: () => Promise.resolve([])
  })
  global.fetch = mockFetch
  
  render(<Vets />)
  
  expect(screen.getByText('Loading...')).toBeInTheDocument()
  
  await waitFor(() => {
    expect(screen.queryByText('Loading...')).not.toBeInTheDocument()
  })
})
```

## Troubleshooting

### Common Issues

1. **"Cannot find module" errors** - Ensure all dependencies are installed
2. **Location mock issues** - The setup handles window.location mocking for hash routing
3. **Async test failures** - Use `await user.click()` for user events
4. **Component not found** - Check that components are properly exported

### Debug Mode
Add `screen.debug()` in tests to see the rendered HTML:

```tsx
test('debug example', () => {
  render(<YourComponent />)
  screen.debug() // Prints current DOM state
})
```

This testing setup provides a solid foundation for ensuring your NetClinic frontend is reliable and well-tested!