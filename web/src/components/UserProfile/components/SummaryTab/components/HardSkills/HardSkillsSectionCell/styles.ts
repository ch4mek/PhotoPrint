import { css, Theme } from '@emotion/react';
import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

const getTextColor = (theme: Theme, isPrimary: boolean) =>
	isPrimary ? theme.palette['text-accent'] : theme.palette['text-black'];

export const CellWrapper = styled.div<{ isPrimary: boolean }>(
	({ theme, isPrimary }) => css`
		height: 100%;
		width: 100%;
		text-overflow: ellipsis;
		overflow: hidden;
		color: ${getTextColor(theme, isPrimary)};
	`
);

export const CellTypography = styled(Typography)<{ isPrimary: boolean }>(
	({ isPrimary, theme }) => css`
		color: ${getTextColor(theme, isPrimary)};
	`
);
