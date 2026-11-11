import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

export const Wrapper = styled('div', {
	shouldForwardProp: (prop) => prop !== 'withDivider',
})<{ withDivider?: boolean }>(({ theme, withDivider = true }) => ({
	padding: `${theme.spaces.m}px ${theme.spaces.l}px`,
	...(withDivider && {
		borderBottom: `1px solid ${theme.palette['graphics-divider-1']}`,
	}),
}));

export const Title = styled(Typography)(({ theme }) => ({
	fontWeight: theme.typography.fontWeightBold,
}));

export const Value = styled(Typography)(({ theme }) => ({
	display: 'block',
	marginTop: theme.spaces.s,
}));
