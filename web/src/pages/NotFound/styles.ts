import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

export const HeaderText = styled(Typography)(({ theme }) => ({
	marginTop: theme.spacing(1.2),
	marginBottom: theme.spacing(2),
	fontSize: theme.typography.pxToRem(40),
	lineHeight: theme.typography.pxToRem(48),
	color: theme.palette['text-black'],
}));

export const MessageText = styled(Typography)(({ theme }) => ({
	marginBottom: theme.spacing(1),
	fontSize: theme.typography.pxToRem(40),
	lineHeight: theme.typography.pxToRem(48),
	color: theme.palette['text-black'],
}));

export const DescriptionText = styled(Typography)(({ theme }) => ({
	fontSize: theme.typography.pxToRem(16),
	lineHeight: theme.typography.pxToRem(24),
	color: theme.palette['text-black'],
}));

export const NotFoundPageContainer = styled.div`
	display: flex;
	width: 90%;
	height: 80vh;
	flex-direction: column;
	justify-content: center;
	align-items: center;
	align-self: center;
	margin: auto;
`;
