import { Theme } from '@emotion/react';

import { TimesheetStatusType } from '../types';

import { RED_TAG_COLOR, GREEN_TAG_COLOR, YELLOW_TAG_COLOR } from '@/theme';

const { Approved, Rejected } = TimesheetStatusType;

export const getTimesheetStatusColor = (theme: Theme, variant: string) => {
	switch (variant) {
		case Approved:
			return GREEN_TAG_COLOR;
		case Rejected:
			return RED_TAG_COLOR;
		default:
			return YELLOW_TAG_COLOR;
	}
};
