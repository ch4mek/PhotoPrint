import styled from '@emotion/styled';

import {
	ColoredMerchCell,
	DisabledColoredMerchCell,
} from '../../../StorageTab/styles';
import {
	EXPANDED_ROWS_HEIGHT,
	FIRST_ROW_CONTENT_MAX_HEIGHT,
} from '../../constants';

import { DEFAULT_ICON_SIZE, SMALL_ICON_SIZE } from '@/constants/commonStyles';

export const SizeCellContentWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.s,
	alignItems: 'center',
	height: '100%',
	overflow: 'hidden',

	'& img': {
		border: `1px solid ${theme.palette['graphics-divider-2']}`,
	},

	'& svg': {
		width: DEFAULT_ICON_SIZE,
		height: DEFAULT_ICON_SIZE,
	},
}));

export const SizeCellWrapper = styled(DisabledColoredMerchCell)`
	display: flex;
	justify-content: space-between;
	height: 100%;
	width: 100%;
	position: relative;
`;

export const SizeFieldsWrapper = styled.div({
	height: '100%',
	width: '100%',
	display: 'flex',
	flexDirection: 'column',
});

export const SizeGeneralFields = styled('div')<{ newHeight?: number }>(
	({ theme, newHeight }) => ({
		width: '100%',
		display: 'flex',
		flexDirection: 'column',
		height: newHeight,
		minHeight: newHeight,
		padding: `${theme.spaces.xs}px ${theme.spaces.s}px`,
		margin: 0,
		boxShadow: `0 1px 0 0 ${theme.palette['graphics-divider-1']}`,
		borderBottom: '1px solid transparent',
	})
);

export const SizeGeneralFieldWrapper = styled('div')<{
	isFirstRow?: boolean;
}>(({ isFirstRow = false }) => ({
	display: 'flex',
	alignItems: 'center',
	justifyContent: 'space-between',
	height: '100%',
	maxHeight: isFirstRow ? FIRST_ROW_CONTENT_MAX_HEIGHT : EXPANDED_ROWS_HEIGHT,

	'& button, button > svg': {
		width: SMALL_ICON_SIZE,
		height: SMALL_ICON_SIZE,
	},
}));

export const SizeSubFields = styled(ColoredMerchCell)(({ theme }) => ({
	display: 'flex',
	height: `${EXPANDED_ROWS_HEIGHT}px`,
	padding: `${theme.spaces.xs}px ${theme.spaces.s}px`,
	borderTop: `1px solid ${theme.palette['graphics-divider-1']}`,
	borderBottom: '1px solid transparent',
	...theme.typography.body['s-400'],
}));

export const FirstRowSizesWrapper = styled('div')(() => ({}));
