import { css } from '@emotion/react';
import styled from '@emotion/styled';
import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { BorderRadiuses } from '@/constants/borderRadiuses';

const MIN_LEFT_COLUMN_WIDTH = 189;
const MIN_RIGHT_COLUMN_WIDTH = 182;
const VIEW_BUTTON_WIDTH = 33;

export const Wrapper = styled('div')<{
	orderName?: string;
}>(
	({ orderName }) => css`
		display: flex;
		flex-direction: column;
		${orderName ? `grid-area: ${orderName}` : ''};
	`
);

export const Title = styled(Typography)(
	({ theme }) => css`
		color: ${theme.palette['text-black']};
	`
);

export const HeaderWrapper = styled('div')(
	({ theme }) => css`
		display: flex;
		align-items: center;
		justify-content: space-between;
		border: 1px solid ${theme.palette['graphics-input-stroke']};
		border-bottom: none;
		border-radius: ${BorderRadiuses.m}px ${BorderRadiuses.m}px 0px 0px;
		background-color: ${theme.palette['background-white']};
		padding: ${theme.spaces.s}px;
	`
);

export const ButtonsWrapper = styled('div')(
	({ theme }) => css`
		display: flex;
		align-items: center;
		gap: ${theme.spaces.s}px;
	`
);

export const ContainerWrapper = styled('div')(({ theme }) => ({
	border: `1px solid ${theme.palette['graphics-input-stroke']}`,
	backgroundColor: theme.palette['background-white'],
	padding: `${theme.spaces.s}px ${theme.spaces.s}px ${theme.spaces.s}px`,
	borderRadius: `0 0 ${BorderRadiuses.m}px ${BorderRadiuses.m}px`,
}));

export const ContentEditWrapper = styled('div')(({ theme }) => ({
	display: 'grid',
	alignItems: 'flex-start',
	columnGap: theme.spaces.xl2,
	rowGap: theme.spaces.m,
	gridTemplateColumns: `minmax(${MIN_LEFT_COLUMN_WIDTH}px, 1fr) minmax(${MIN_RIGHT_COLUMN_WIDTH}px, 1fr)`,

	[`${theme.breakpoints.down(theme.breakpoints.tabletS)}`]: {
		display: 'flex',
		flexDirection: 'column',
		gap: theme.spaces.m,
	},
}));

export const ContentColumn = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	alignItems: 'flex-start',
	justifyContent: 'flex-start',
	wordBreak: 'break-all',
	gap: theme.spaces.m,
	width: '100%',
}));

export const ViewButton = styled(ActionButton)({
	width: VIEW_BUTTON_WIDTH,
});
