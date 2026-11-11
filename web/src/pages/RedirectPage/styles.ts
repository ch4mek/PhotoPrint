import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

export const PageContainer = styled('div')(({ theme }) => ({
	display: 'flex',
	backgroundColor: theme.palette['background-base-1'],
	justifyContent: 'center',
	alignContent: 'center',
	height: '100vh',
	position: 'relative',
	padding: 0,
	overflow: 'hidden',
}));

export const Wrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	maxWidth: 'fit-content',
	alignItems: 'center',
	justifyContent: 'center',
	flexDirection: 'column',
	gap: theme.spaces.m,
}));

export const TypographyWrapper = styled('div')(({ theme }) => ({
	textAlign: 'center',
	color: theme.palette['text-black'],
}));

export const ResourceTypography = styled(Typography)(({ theme }) => ({
	color: theme.palette['text-accent'],
}));
