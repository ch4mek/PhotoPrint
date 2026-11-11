import styled from '@emotion/styled';

import { DisabledColoredMerchCell } from '../../../StorageTab/styles';
import {
	EXPANDED_ROWS_HEIGHT,
	EXPANDED_STORAGE_HEIGHT,
	FIRST_ROW_CONTENT_MAX_HEIGHT,
} from '../../constants';

import { DEFAULT_ICON_SIZE, SMALL_ICON_SIZE } from '@/constants/commonStyles';

export const StorageCellContentWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.s,
	alignItems: 'center',
	height: '100%',
	overflow: 'hidden',
	'& svg': {
		width: DEFAULT_ICON_SIZE,
		height: DEFAULT_ICON_SIZE,
	},
}));

export const StorageCellWrapper = styled(DisabledColoredMerchCell)`
	display: flex;
	justify-content: space-between;
	height: 100%;
	width: 100%;
	position: relative;
`;

export const StorageFieldsWrapper = styled.div({
	height: '100%',
	width: '100%',
	display: 'flex',
	flexDirection: 'column',
});

export const StorageGeneralFields = styled('div')<{ newHeight?: number }>(
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

export const StorageGeneralFieldWrapper = styled('div')<{
	isFirstRow?: boolean;
}>(({ isFirstRow = false }) => ({
	display: 'flex',
	alignItems: 'center',
	justifyContent: 'space-between',
	height: '100%',
	overflowX: 'hidden',
	maxHeight: isFirstRow ? FIRST_ROW_CONTENT_MAX_HEIGHT : EXPANDED_ROWS_HEIGHT,

	'& button, button > svg': {
		width: SMALL_ICON_SIZE,
		height: SMALL_ICON_SIZE,
	},
}));

export const StorageSubFields = styled('div')<{ isClothing?: boolean }>(
	({ theme, isClothing = true }) => ({
		display: 'flex',
		height: `${isClothing ? EXPANDED_STORAGE_HEIGHT : EXPANDED_ROWS_HEIGHT}px`,
		padding: isClothing
			? theme.spaces.s
			: `${theme.spaces.xs}px ${theme.spaces.s}px`,
		boxShadow: `0 1px 0 0 ${theme.palette['graphics-divider-1']}`,
		borderBottom: '1px solid transparent',
	})
);
