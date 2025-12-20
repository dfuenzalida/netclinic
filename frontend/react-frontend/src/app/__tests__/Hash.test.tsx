import { renderHook, act } from '@testing-library/react'
import useHash from '../components/Hash'

describe('useHash hook', () => {
  beforeEach(() => {
    // Reset location.hash before each test
    window.location.hash = ''
    
    // Mock event listeners
    window.addEventListener = jest.fn()
    window.removeEventListener = jest.fn()
  })

  test('initializes with empty hash', () => {
    const { result } = renderHook(() => useHash())
    const [hash] = result.current
    
    expect(hash).toBe('')
  })

  test('updates hash when updateHash is called', () => {
    const { result } = renderHook(() => useHash())
    
    act(() => {
      const [, updateHash] = result.current
      updateHash('#new-hash')
    })
    
    expect(window.location.hash).toBe('#new-hash')
  })

  test('sets up hashchange event listener', () => {
    renderHook(() => useHash())
    
    expect(window.addEventListener).toHaveBeenCalledWith(
      'hashchange',
      expect.any(Function)
    )
  })
})