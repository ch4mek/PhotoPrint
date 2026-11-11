import { css, SerializedStyles, Theme } from '@emotion/react';
import styled from '@emotion/styled';

import { PlanCompletionStatus } from '../constants';

import { NBSP } from '@/pages/Payload/contants';
import { ThemeMode } from '@/theme';

import {
	FirstWorkloadPartProps,
	BaseWorkloadProps,
	SecondWorkloadPartProps,
	TypeOfTimePart,
} from './types';

const TIMELINE_PADDING = 2;

const WorkloadPartBase = styled('span')({
	display: 'inline-block',
	overflow: 'hidden',
	textOverflow: 'ellipsis',
	textAlign: 'right',
});

const getFirstWorkloadColorBasedOnCompletion = (
	theme: Theme
): Record<PlanCompletionStatus, SerializedStyles> => ({
	[PlanCompletionStatus.Zero]: css({
		color: theme.palette['text-black'],
	}),
	[PlanCompletionStatus.NotCompleted]: css({
		color: theme.palette['feedback-danger'],
	}),
	[PlanCompletionStatus.Completed]: css({
		color: theme.palette['feedback-success'],
	}),
	[PlanCompletionStatus.Exceeded]: css({
		color: theme.palette['text-violet'],
	}),
});

const getTextColor = (theme: Theme, isPrimaryValue: boolean) => {
	if (theme.mode === ThemeMode.LIGHT) {
		return theme.palette[isPrimaryValue ? 'text-black' : 'text-gray'];
	}

	return theme.palette['text-white'];
};

const getMainTypeCellStyles = (
	theme: Theme,
	isSubRow: boolean,
	isPrimaryValue = true
) => ({
	...theme.typography.body[isSubRow || !isPrimaryValue ? 's-600' : 'm-700'],
	color: getTextColor(theme, isPrimaryValue),
});

export const FirstWorkloadPart = styled(
	WorkloadPartBase
)<FirstWorkloadPartProps>(
	({ theme, type, planCompletion, isSubRow }) =>
		({
			main: css(getMainTypeCellStyles(theme, !!isSubRow)),
			secondary: [
				{
					...theme.typography.body[isSubRow ? 'xs-400' : 's-600'],
				},
				getFirstWorkloadColorBasedOnCompletion(theme)[planCompletion],
			],
			total: css({
				...theme.typography.body['xs-400'],
				color: theme.palette['text-black'],
			}),
		}[type])
);

export const SecondWorkloadPart = styled(
	WorkloadPartBase
)<SecondWorkloadPartProps>(
	({ type, theme, isPrimaryValue, isSubRow, planCompletion }) => {
		const secondaryTextStyle = [
			{
				...theme.typography.body[isSubRow ? 'xs-400' : 's-600'],
				color: theme.palette['text-gray'],
			},
			isPrimaryValue && planCompletion === PlanCompletionStatus.Completed
				? getFirstWorkloadColorBasedOnCompletion(theme)[planCompletion]
				: undefined,
		];

		return [
			{
				main: css(getMainTypeCellStyles(theme, !!isSubRow, isPrimaryValue)),
				secondary: secondaryTextStyle,
				total: css({
					...theme.typography.body['xs-400'],
					color: theme.palette['text-gray'],
				}),
			}[type],
			{
				'::before': {
					display: isPrimaryValue ? 'none' : 'inline',
					content: type === TypeOfTimePart.Total ? '"/"' : `"${NBSP}/${NBSP}"`,
				},
			},
		];
	}
);

export const TimePartWrapper = styled('div')<BaseWorkloadProps>(
	({ type, disabled }) => ({
		padding: `0px ${type === TypeOfTimePart.Total ? 0 : TIMELINE_PADDING}px`,
		display: 'flex',
		alignItems: 'baseline',
		width: '100%',
		justifyContent: `${type === TypeOfTimePart.Total ? 'flex-end' : 'center'}`,
		pointerEvents: disabled ? 'none' : 'auto',
	})
);
