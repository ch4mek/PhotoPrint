export const removeLastLetterAccordingToAmount = (
	word: string,
	amount: number
) => (amount === 1 ? word.slice(0, -1) : word);
