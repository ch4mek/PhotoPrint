import { css } from '@emotion/react';
import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { BorderRadiuses } from '@/constants/borderRadiuses';

const IMAGE_SIZE = 68;
const NEED_A_PROJECT_TOGGLE_WIDTH = 119;

export const PersonalInfoWrapper = styled('div')(
	({ theme }) => css`
		display: flex;
		flex-direction: column;
		justify-content: flex-start;
		align-items: flex-start;
		gap: ${theme.spaces.s}px;
		width: 100%;
		min-width: 0;
		max-width: calc(100% - ${IMAGE_SIZE}px - ${theme.spaces.xl}px);
	`
);

export const PersonalInfoTopRow = styled('div')(
	({ theme }) => css`
		display: flex;
		flex-flow: row nowrap;
		align-items: center;
		gap: ${theme.spaces.m}px;
		width: 100%;
		min-width: 0;

		& > *:only-child {
			width: 100%;
		}

		& > *:first-child:not(:only-child) {
			flex: 1;
			min-width: 0;
		}
	`
);

export const PersonalInfoHeaderActionsWrapper = styled('div')(
	() => css`
		flex-shrink: 0;
		display: flex;
		align-items: center;
	`
);

export const NameAndSeniorityWrapper = styled('div')(({ theme }) => ({
	maxWidth: '100%',
	width: '100%',
	display: 'flex',
	justifyContent: 'flex-start',
	alignItems: 'flex-start',
	gap: theme.spaces.m,

	[theme.breakpoints.down(theme.breakpoints.desktopXS)]: {
		flexDirection: 'column',
		gap: theme.spaces.s,
		width: '100%',
	},
}));

export const NameWrapper = styled('div')<{ withNeedAProjectButton: boolean }>(
	({ theme, withNeedAProjectButton }) => ({
		maxWidth: withNeedAProjectButton
			? `calc(100% - ${NEED_A_PROJECT_TOGGLE_WIDTH}px - ${theme.spaces.m}px)`
			: '100%',
		display: 'flex',
		flexDirection: 'column',
		justifyContent: 'flex-start',
		alignItems: 'flex-start',
		gap: theme.spaces.xs,

		[theme.breakpoints.down(theme.breakpoints.desktopXS)]: {
			maxWidth: '100%',
		},
	})
);

export const FixedTypography = styled(Typography)(
	({ theme }) => css`
		max-width: 100%;
		overflow: hidden;
		text-overflow: ellipsis;
		white-space: nowrap;
		gap: ${theme.spaces.m}px;
		color: ${theme.palette['text-black']};
		display: block;
		flex-shrink: 1;
	`
);

export const SpecializationName = styled(FixedTypography)(({ theme }) => ({
	color: theme.palette['text-accent'],
}));

export const NeedAProjectTypography = styled(Typography)(
	() => css`
		white-space: nowrap;
	`
);

export const EmailTypography = styled(Typography)(
	({ theme }) => css`
		width: 100%;
		overflow: hidden;
		text-overflow: ellipsis;
		white-space: nowrap;
		color: ${theme.palette['text-gray']};
	`
);

export const InfoWrapper = styled('div')(
	({ theme }) => css`
		width: 100%;
		display: flex;
		justify-content: flex-start;
		align-items: center;
		gap: ${theme.spaces.m}px;
	`
);

export const Wrapper = styled('div')<{ isNeedWrapperStyles?: boolean }>(
	({ theme, isNeedWrapperStyles }) => css`
		display: flex;
		justify-content: flex-start;
		align-items: flex-start;
		padding: ${theme.spaces.s}px;
		background-color: ${isNeedWrapperStyles
			? theme.palette['background-base-2']
			: 'none'};
		border-radius: ${isNeedWrapperStyles ? `${BorderRadiuses.m}px` : 'none'};
		outline: ${isNeedWrapperStyles
			? `1px solid ${theme.palette['graphics-divider-1']}`
			: 'none'};
	`
);

export const CommentSection = styled('div')(
	() => css`
		flex-shrink: 0;
		width: 100%;
	`
);
