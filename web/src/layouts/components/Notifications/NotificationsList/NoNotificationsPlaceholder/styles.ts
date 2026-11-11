import { css } from '@emotion/react';
import styled from '@emotion/styled';

export const NoNotificationsPlaceholderWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	alignItems: 'center',
	color: theme.palette['text-black'],
}));

export const BellIconWrapper = styled('div')(
	({ theme }) => css`
		color: ${theme.palette['graphics-primary-2']};
	`
);
