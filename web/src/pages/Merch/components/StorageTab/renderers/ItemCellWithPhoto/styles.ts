import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { ColoredMerchCell } from '../../styles';

import { DEFAULT_ICON_SIZE, SMALL_ICON_SIZE } from '@/constants/commonStyles';
import {
	EXPANDED_ROWS_HEIGHT,
	FIRST_ROW_CONTENT_MAX_HEIGHT,
} from '@/pages/Merch/components/ElementsTab/constants';

export const ItemCellWithPhotoContentWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.s,
	alignItems: 'center',
	overflow: 'hidden',
	width: '100%',

	'& svg': {
		width: DEFAULT_ICON_SIZE,
		height: DEFAULT_ICON_SIZE,
	},
}));

export const NameCellWrapper = styled(ColoredMerchCell)`
	display: flex;
	justify-content: space-between;
	height: 100%;
	width: 100%;
	position: relative;
`;

export const NameFieldsWrapper = styled.div({
	height: '100%',
	width: '100%',
	display: 'flex',
	flexDirection: 'column',
});

export const NameGeneralFields = styled('div')<{
	newHeight?: number;
}>(({ theme, newHeight }) => ({
	width: '100%',
	flexDirection: 'column',
	height: newHeight,
	minHeight: newHeight,
	padding: `${theme.spaces.xs}px ${theme.spaces.s}px`,
	margin: 0,
}));

export const NameGeneralFieldWrapper = styled('div')<{ isFirstRow?: boolean }>(
	({ isFirstRow = false }) => ({
		display: 'flex',
		alignItems: 'center',
		justifyContent: 'space-between',
		maxHeight: isFirstRow ? FIRST_ROW_CONTENT_MAX_HEIGHT : EXPANDED_ROWS_HEIGHT,

		'& button, button > svg': {
			width: SMALL_ICON_SIZE,
			height: SMALL_ICON_SIZE,
		},
	})
);

export const NameTypography = styled(Typography)(() => ({
	textOverflow: 'ellipsis',
	overflow: 'hidden',
}));
