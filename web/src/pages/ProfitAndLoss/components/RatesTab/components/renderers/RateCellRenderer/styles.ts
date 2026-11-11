import styled from '@emotion/styled';

import { CellWrapper } from '@/components/Grid';

import {
	RATE_BYN_CHILD_CURRENCY_FONT_SIZE,
	RATE_BYN_PARENT_CURRENCY_FONT_SIZE,
	RATE_CHILD_CELL_VALUE_GAP,
	RATE_CHILD_CURRENCY_LINE_HEIGHT,
	RATE_PARENT_VALUE_LINE_HEIGHT,
} from './constants';

export const RateCellContent = styled(CellWrapper)({
	display: 'flex',
	alignItems: 'center',
	justifyContent: 'flex-end',
	minWidth: 0,
	height: '100%',
	width: '100%',
});

export const RateCellValue = styled('span')<{ isChildRow?: boolean }>(
	({ theme, isChildRow }) => ({
		display: 'inline-flex',
		alignItems: 'baseline',
		justifyContent: 'flex-end',
		gap: isChildRow ? RATE_CHILD_CELL_VALUE_GAP : theme.spaces.xs,
		minWidth: 0,
		maxWidth: '100%',
	})
);

export const RateCurrencyPart = styled('span')<{
	isChildRow?: boolean;
	isByn?: boolean;
}>(({ theme, isChildRow, isByn }) => ({
	flexShrink: 0,
	textAlign: 'right',
	color: theme.palette['text-gray'],
	...(isChildRow
		? {
				...theme.typography.body['xs-400'],
				lineHeight: RATE_CHILD_CURRENCY_LINE_HEIGHT,
				...(isByn && {
					fontSize: RATE_BYN_CHILD_CURRENCY_FONT_SIZE,
				}),
		  }
		: {
				...theme.typography.body['s-400'],
				...(isByn && {
					fontSize: RATE_BYN_PARENT_CURRENCY_FONT_SIZE,
				}),
		  }),
}));

export const RateValuePart = styled('span')<{ isChildRow?: boolean }>(
	({ theme, isChildRow }) => ({
		flexShrink: 0,
		textAlign: 'right',

		...(isChildRow
			? theme.typography.body['s-400']
			: {
					...theme.typography.body['m-400'],
					lineHeight: RATE_PARENT_VALUE_LINE_HEIGHT,
			  }),
	})
);
