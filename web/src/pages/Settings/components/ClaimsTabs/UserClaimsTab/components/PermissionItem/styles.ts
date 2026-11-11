import { css } from '@emotion/react';
import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { BorderRadiuses } from '@/constants/borderRadiuses';

const MIN_WIDTH_PERMISION_ITEM = 215;

export const PermissionWrapper = styled('div')(
	({ theme }) => css`
		border: 1px solid ${theme.palette['graphics-divider-2']};
		border-radius: ${BorderRadiuses.m}px;
		padding: ${theme.spaces.m}px;
		display: flex;
		flex-direction: column;
		gap: ${theme.spaces.s}px;
		min-width: ${MIN_WIDTH_PERMISION_ITEM}px;
	`
);

export const PermissionTitle = styled(Typography)(
	({ theme }) => css`
		color: ${theme.palette['text-gray']};
	`
);

export const Permissions = styled('div')(
	({ theme }) => css`
		color: ${theme.palette['text-black']};
		display: flex;
		flex-direction: column;
		gap: ${theme.spaces.s}px;
	`
);

export const ClaimTitle = styled(Typography)(() => ({
	maxWidth: '100%',
	overflow: 'hidden',
	whiteSpace: 'nowrap',
	textOverflow: 'ellipsis',
}));

export const CheckboxWrapper = styled('div')(() => ({
	overflow: 'hidden',
	whiteSpace: 'nowrap',
	textOverflow: 'ellipsis',
	width: '100%',

	'& label:has(p)': {
		width: '85%',
	},
}));
