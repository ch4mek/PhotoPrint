import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { FIRST_ROW_CONTENT_MAX_HEIGHT } from '../../constants';

import { DEFAULT_ICON_SIZE, SMALL_ICON_SIZE } from '@/constants/commonStyles';

export const ItemCellWithPhotoContentWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.s,
	alignItems: 'center',
	overflow: 'hidden',
	flex: 1,

	'& svg': {
		width: DEFAULT_ICON_SIZE,
		height: DEFAULT_ICON_SIZE,
	},
}));

export const NameCellWrapper = styled('div')({
	display: 'flex',
	justifyContent: 'space-between',
	height: '100%',
	width: '100%',
	position: 'relative',
});

export const NameFieldsWrapper = styled('div')({
	height: '100%',
	width: '100%',
	display: 'flex',
	flexDirection: 'column',
});

export const NameGeneralFields = styled('div')<{ newHeight?: number }>(
	({ theme, newHeight }) => ({
		width: '100%',
		flexDirection: 'column',
		height: newHeight,
		minHeight: newHeight,
		padding: `${theme.spaces.xs}px ${theme.spaces.s}px`,
		margin: 0,
	})
);

export const NameGeneralFieldWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	justifyContent: 'space-between',
	gap: theme.spaces.s,
	maxHeight: FIRST_ROW_CONTENT_MAX_HEIGHT,

	'& button, button > svg': {
		width: SMALL_ICON_SIZE,
		height: SMALL_ICON_SIZE,
	},
}));

export const NameTypography = styled(Typography)(() => ({
	textOverflow: 'ellipsis',
	overflow: 'hidden',
	flex: 1,
}));
