import { css } from '@emotion/react';
import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

export const ItemWrapper = styled('div')(
	({ theme }) => css`
		display: flex;
		flex-direction: column;
		align-items: flex-start;
		gap: ${theme.spaces.xs}px;
		width: 100%;
	`
);

export const FixedTypography = styled(Typography)(
	({ theme }) => css`
		color: ${theme.palette['text-black']};
		width: 100%;
		text-overflow: ellipsis;
		white-space: nowrap;
		overflow: hidden;
		display: block;
	`
);

export const NameOfFiledTypography = styled(Typography)(
	({ theme }) => css`
		color: ${theme.palette['text-gray']};
	`
);
