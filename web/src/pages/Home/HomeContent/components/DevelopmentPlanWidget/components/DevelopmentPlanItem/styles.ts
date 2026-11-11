import { css } from '@emotion/react';
import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { ThemeMode } from '@/theme';

const MAX_ROW_COUNT = 3;
const DEVELOPMENT_PLAN_ITEM_MAX_HEIGHT = 150;

export const DevelopmentPlanItemWrapper = styled('li')<{
	isDesktopView?: boolean;
}>(({ isDesktopView = true }) => ({
	listStyle: 'none',
	width: '100%',
	height: 'fit-content',
	maxHeight: isDesktopView ? DEVELOPMENT_PLAN_ITEM_MAX_HEIGHT : 'none',
}));

export const PlanItem = styled('article')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,
	width: '100%',
	height: '100%',
	padding: theme.spaces.s,

	backgroundColor:
		theme.mode === ThemeMode.LIGHT
			? theme.palette['background-base-1']
			: theme.palette['background-base-2'],
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	borderRadius: BorderRadiuses.m,
}));

const maxHeightStyles = (maxHeight: number) => css`
	display: -webkit-box;
	line-clamp: ${maxHeight};
	-webkit-line-clamp: ${maxHeight};
	-webkit-box-orient: vertical;
	text-overflow: ellipsis;
	overflow: hidden;
	white-space: pre-wrap;
`;
export const PlanSummaryWrapper = styled('div')(
	({ theme }) => css`
		width: 100%;
		height: fit-content;
		color: ${theme.palette['text-black']};
		overflow-wrap: break-word;
		word-break: break-word;
		${maxHeightStyles(MAX_ROW_COUNT)};
	`
);

export const PlanTypography = styled(Typography)({
	display: 'block',
});

export const ControlWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'row',
	justifyContent: 'space-between',
	alignItems: 'center',
	color: theme.palette['text-gray'],
	marginTop: 'auto',
}));
