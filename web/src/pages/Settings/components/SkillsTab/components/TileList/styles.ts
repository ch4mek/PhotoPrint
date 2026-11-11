import { css } from '@emotion/react';
import styled from '@emotion/styled';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { TILE_LIST_ITEM_HEIGHT } from '../../constants';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { scrollbarWidths } from '@/constants/commonStyles';

const totalCountSize = '26px';
const TITLE_SIZES = {
	s: 7,
	m: 40,
};

export const TileListWrapper = styled('div')(
	() => css`
		border-radius: ${BorderRadiuses.m}px;
		height: 100%;
		display: flex;
		flex-direction: column;
		width: 100%;
	`
);

export const TileListHeaderWrapper = styled('div')<{ beforeColor: string }>(
	({ theme, beforeColor }) => css`
		border-radius: ${BorderRadiuses.m}px;
		border: 1px solid ${theme.palette['graphics-input-stroke']};
		background-color: ${theme.palette['background-white']};
		overflow: hidden;
		height: ${TILE_LIST_ITEM_HEIGHT}px;
		display: flex;
		align-items: center;
		justify-content: space-between;
		position: relative;
		padding: 0 ${theme.spaces.m}px;
		gap: ${theme.spaces.s}px;
		flex: 0 0 auto;

		&::before {
			content: '';
			left: 0;
			width: ${TITLE_SIZES.s}px;
			position: absolute;
			height: 100%;
			background-color: ${beforeColor};
		}
	`
);

export const TileListHeaderColorWrapper = styled('div')(
	({ theme }) => css`
		border-top-right-radius: ${BorderRadiuses.m}px;
		border-top-left-radius: ${BorderRadiuses.m}px;
		background-color: ${theme.palette['background-base-3']};
	`
);

export const TileListControls = styled('div')(
	({ theme }) => css`
		display: flex;
		align-items: center;
		gap: ${theme.spaces.s}px;
	`
);

export const TotalCountContainer = styled('div')(
	({ theme }) => css`
		display: flex;
		justify-content: center;
		align-items: center;
		border-radius: ${BorderRadiuses.half};
		border: 1px solid ${theme.palette['graphics-divider-1']};
		width: ${totalCountSize};
		height: ${totalCountSize};
	`
);

export const ScrollbarWrapper = styled('div')(({ theme }) => ({
	...getScrollbarStyles(theme, scrollbarWidths.s, 'vertical'),
	display: 'flex',
	gap: `${theme.spaces.xs}px`,
	flexDirection: 'column',
	padding: `0 ${theme.spaces.s}px`,
	height: '100%',
	maxHeight: '100%',
	'& > div': {
		flexShrink: 0,
	},
	'&::-webkit-scrollbar-track': {
		backgroundColor: 'transparent',
	},
}));

export const TileListBody = styled('div')<{ containsLoader: boolean }>(
	({ theme, containsLoader }) => css`
		padding: ${theme.spaces.s}px 0;
		height: ${containsLoader ? '100%' : 'fit-content'};
		background-color: ${theme.palette['background-base-3']};
		border-bottom-left-radius: ${BorderRadiuses.m}px;
		border-bottom-right-radius: ${BorderRadiuses.m}px;

		max-height: calc(100% - ${TITLE_SIZES.m}px);
	`
);

export const TileListContent = styled('div')<{ height: string }>(
	({ height }) => css`
		position: relative;
		height: ${height};
	`
);

export const NoItemsMessageWrapper = styled(Typography)(
	({ theme }) => css`
		display: block;
		color: ${theme.palette['text-black']};
	`
);

export const TotalTypography = styled(Typography)(
	({ theme }) => css`
		color: ${theme.palette['text-gray']};
	`
);

export const TitleTypography = styled(Typography)(
	({ theme }) => css`
		color: ${theme.palette['text-black']};
	`
);
