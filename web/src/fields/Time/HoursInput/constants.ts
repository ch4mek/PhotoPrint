// eslint-disable-next-line import/no-extraneous-dependencies
import { MaskedRange } from 'imask';

export const maskBlocks = {
	HHHH: {
		mask: MaskedRange,
		maxLength: 4,
		from: 0,
		to: 9999,
	},
};
