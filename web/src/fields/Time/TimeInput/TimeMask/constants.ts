// eslint-disable-next-line import/no-extraneous-dependencies
import IMask from 'imask';

export const maskBlocks = {
	HH: {
		mask: IMask.MaskedRange,
		maxLength: 2,
		from: 0,
		to: 23,
		autofix: 'pad',
	},
	MM: {
		mask: IMask.MaskedRange,
		from: 0,
		to: 59,
		maxLength: 2,
	},
};
