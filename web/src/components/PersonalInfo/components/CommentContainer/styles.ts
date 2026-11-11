import { css } from '@emotion/react';
import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

export const CommentWrapper = styled('div')(
	({ theme }) => css`
		display: flex;
		justify-content: flex-start;
		align-items: center;
		gap: ${theme.spaces.m}px;
	`
);

export const Placeholder = styled(Typography)(
	({ theme }) => css`
		color: ${theme.palette['text-gray']};
	`
);

export const FixedTypography = styled(Typography)(
	({ theme }) => css`
		overflow-wrap: break-word;
		word-break: break-word;
		white-space: normal;
		color: ${theme.palette['text-black']};
	`
);
