import styled from '@emotion/styled';

import { HOME_MOBILE_BREAKPOINT } from '../../constants';

import { NUMBER_OF_CARDS_FOR_FULL_VIEW } from './constants';

const DEFAULT_CARD_HEIGHT = 136;
const MOBILE_CARD_HEIGHT = 54;

export const BirthdaysWidgetWrapper = styled('div')<{
	countOfCards: number;
}>(({ countOfCards, theme }) => ({
	display: 'grid',
	gap: theme.spaces.s,
	gridTemplateColumns:
		countOfCards <= NUMBER_OF_CARDS_FOR_FULL_VIEW ? '1fr' : 'repeat(2, 1fr)',
	width: '100%',
	gridAutoRows: DEFAULT_CARD_HEIGHT,
	height: 'fit-content',

	[theme.breakpoints.down(HOME_MOBILE_BREAKPOINT)]: {
		gridTemplateColumns: '1fr',
		gridAutoRows: MOBILE_CARD_HEIGHT,
	},
}));
