import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

const SUMMARY_LABEL_WIDTH = 30;
const SUMMARY_VALUE_WIDTH = 80;

export const TimeCellWrapper = styled('div')<{ isMinWorkingTime: boolean }>(
	({ theme, isMinWorkingTime }) => ({
		color: isMinWorkingTime
			? theme.palette['feedback-success']
			: theme.palette['feedback-danger'],
	})
);

export const SummaryRowContainer = styled('div')({
	display: 'flex',
	alignItems: 'center',
	justifyContent: 'end',
	width: '100%',
});

export const SummaryLabel = styled(Typography)(({ theme }) => ({
	color: theme.palette['text-black'],
	width: SUMMARY_LABEL_WIDTH,
	textAlign: 'left',
}));

export const SummaryValue = styled(Typography)(({ theme }) => ({
	color: theme.palette['text-black'],
	width: SUMMARY_VALUE_WIDTH,
	textAlign: 'right',
}));
