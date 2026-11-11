import { Theme } from '@emotion/react';
import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

const getSectionDividerStyles = (theme: Theme) => ({
	borderBottom: `1px solid ${theme.palette['graphics-divider-1']}`,
	padding: `${theme.spaces.m}px ${theme.spaces.l}px`,
});

export const Wrapper = styled('div')(({ theme }) => ({
	...getSectionDividerStyles(theme),
	display: 'flex',
	width: '100%',
	gap: theme.spaces.l,
}));

export const Column = styled('div')(() => ({
	display: 'flex',
	flexDirection: 'column' as const,
	flex: 1,
}));

export const Value = styled(Typography)(({ theme }) => ({
	marginTop: theme.spaces.xs,
}));
