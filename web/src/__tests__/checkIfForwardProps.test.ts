import { checkIfForwardProps } from '@/helpers/checkIfForwardProps';

describe('Validation function checkIfForwardProps', () => {
	it('Should return true when propNames is a string and prop is different', () => {
		const propNames = 'propA';
		const prop = 'propB';
		const result = checkIfForwardProps(propNames)(prop);
		expect(result).toBe(true);
	});

	it('Should return false when propNames is a string and prop is the same', () => {
		const propNames = 'propA';
		const prop = 'propA';
		const result = checkIfForwardProps(propNames)(prop);
		expect(result).toBe(false);
	});

	it('Should return true when propNames is an array and prop is not in the array', () => {
		const propNames = ['propA', 'propB'];
		const prop = 'propC';
		const result = checkIfForwardProps(propNames)(prop);
		expect(result).toBe(true);
	});

	it('Should return false when propNames is an array and prop is in the array', () => {
		const propNames = ['propA', 'propB'];
		const prop = 'propA';
		const result = checkIfForwardProps(propNames)(prop);
		expect(result).toBe(false);
	});

	it('Should work with an array of length 1', () => {
		const propNames = ['propA'];
		const prop = 'propB';
		const result = checkIfForwardProps(propNames)(prop);
		expect(result).toBe(true);
	});

	it('Should work with an array of length 1 and matching prop', () => {
		const propNames = ['propA'];
		const prop = 'propA';
		const result = checkIfForwardProps(propNames)(prop);
		expect(result).toBe(false);
	});

	it('Should work with multiple propNames as a string', () => {
		const propNames = 'propA';
		const prop = 'propB';
		const result = checkIfForwardProps(propNames)(prop);
		expect(result).toBe(true);
	});

	it('Should work with multiple propNames as an array', () => {
		const propNames = ['propA', 'propB'];
		const prop = 'propC';
		const result = checkIfForwardProps(propNames)(prop);
		expect(result).toBe(true);
	});
});
