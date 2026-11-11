import { assertNever } from '@/helpers/assertNever';

describe('assertNever', () => {
	it('should throw error with the unexpected value', () => {
		const unexpectedValue = 'unexpected' as never;

		expect(() => assertNever(unexpectedValue)).toThrow(
			'Unexpected value: unexpected'
		);
	});

	it('should throw error with number value', () => {
		const unexpectedValue = 42 as never;

		expect(() => assertNever(unexpectedValue)).toThrow('Unexpected value: 42');
	});

	it('should throw error with object value', () => {
		const unexpectedValue = { type: 'unknown' } as never;

		expect(() => assertNever(unexpectedValue)).toThrow(
			'Unexpected value: [object Object]'
		);
	});

	it('should throw an Error instance', () => {
		const unexpectedValue = 'test' as never;

		expect(() => assertNever(unexpectedValue)).toThrow(Error);
	});
});
