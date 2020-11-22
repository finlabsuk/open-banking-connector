
// Typescript 3.7 compatible assert
export function assert(condition: unknown, msg?: string): asserts condition {
  if (!condition) {
      throw new Error(msg)
  }
}

// Typescript 3.7 compatible assert
export function assertIsDefined<T>(val: T): asserts val is NonNullable<T> {
  if (val === undefined || val === null) {
      throw new Error(
          `Expected 'val' to be defined, but received ${val}`
      )
  }
}