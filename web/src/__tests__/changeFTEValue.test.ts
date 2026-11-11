import { changeFTEValue } from '@/forms/ProjectAccessAddingBase/components/FTEInput/utils';

describe('changeFTEValue', () => {
	const onChangeMock = jest.fn();

	beforeEach(() => {
		jest.clearAllMocks();
	});

	it('should change valid values', () => {
		const validValues = ['0.11', '0.5', '0.6', '0', '1'];

		validValues.forEach((value) => {
			changeFTEValue(value, onChangeMock);
		});

		expect(onChangeMock).toHaveBeenCalledTimes(validValues.length);
	});

	it('should not change invalid values', () => {
		const invalidValues = ['asdasdas', '0.1.1', '000000000', '0.,1'];

		invalidValues.forEach((value) => {
			changeFTEValue(value, onChangeMock);
		});

		expect(onChangeMock).toHaveBeenCalledTimes(0);
	});

	it('should change integer numbers', () => {
		const validValues: Array<{
			value: string;
			expectedChange: string;
		}> = [
			{
				value: '9',
				expectedChange: '0.9',
			},
			{
				value: '01111',
				expectedChange: '0.11',
			},
			{
				value: '666',
				expectedChange: '0.66',
			},
			{
				value: '2',
				expectedChange: '0.2',
			},
		];

		validValues.forEach(({ value, expectedChange }) => {
			changeFTEValue(value, onChangeMock);
			expect(onChangeMock).toHaveBeenLastCalledWith(expectedChange);
		});
	});
});
