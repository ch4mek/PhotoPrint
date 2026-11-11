import { css } from '@emotion/react';
import styled from '@emotion/styled';
import { Avatar } from '@mms/mms-ui-library/components/Avatar';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { zIndexes } from '@/constants/zIndexes';

const AVATAR_SIZE = 68;
const MAX_WIDTH_SET_PROFILE = 400;
const MAX_HEIGHT_SET_PROFILE = 434;

export const CustomAvatar = styled(Avatar)(
	({ theme }) => css`
		font-weight: 700;
		font-size: ${theme.typography.pxToRem(16)};
		line-height: ${theme.typography.pxToRem(24)};
		width: ${AVATAR_SIZE}px;
		height: ${AVATAR_SIZE}px;
		min-width: ${AVATAR_SIZE}px;

		& img {
			width: ${AVATAR_SIZE}px;
			height: ${AVATAR_SIZE}px;
		}
	`
);

export const SetProfileWrapper = styled('div')(
	({ theme }) => css`
		height: 100%;
		width: 100%;
		display: flex;
		align-items: center;
		justify-content: center;
		font-size: ${theme.typography.pxToRem(16)};
	`
);

export const SetProfilePanel = styled('div')(
	({ theme }) => css`
		width: 100%;
		display: flex;
		flex-direction: column;
		gap: ${theme.spaces.s}px;
		max-width: ${MAX_WIDTH_SET_PROFILE}px;
		max-height: ${MAX_HEIGHT_SET_PROFILE}px;
		padding: ${theme.spaces.m}px;
		border-radius: ${BorderRadiuses.m}px;
		border: 1px solid ${theme.palette['graphics-divider-1']};
		background-color: ${theme.palette['background-white']};
		z-index: ${zIndexes.formPopup};
	`
);
