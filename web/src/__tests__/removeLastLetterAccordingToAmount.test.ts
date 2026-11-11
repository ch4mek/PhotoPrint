import { removeLastLetterAccordingToAmount } from '@/helpers/removeLastLetterAccordingToAmount';

describe('Helper function removeLastLetterAccordingToAmount', () => {
	it('Should remove the last letter if amount = 1', () => {
		expect(removeLastLetterAccordingToAmount('days', 1)).toBe('day');
		expect(removeLastLetterAccordingToAmount('hours', 1)).toBe('hour');
	});

	it('Shouldnt modify the word if amount ≠ 1', () => {
		expect(removeLastLetterAccordingToAmount('days', 2)).toBe('days');
		expect(removeLastLetterAccordingToAmount('hours', 0)).toBe('hours');
	});
});
