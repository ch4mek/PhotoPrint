import { Theme } from '@emotion/react';
import { ReactElement } from 'react';

import { upcomingEventsOptions } from '../constants';

import { iconStyles, noIconStyles } from '@/constants/absence';
import { ThemeMode } from '@/theme';
import { AbsenceType } from '@/types/absences';
import { UpcomingEvent, UpcomingEventType } from '@/types/home';

interface EventConfig {
	icon: ReactElement;
	color: string;
	background: string;
	title?: string | null;
}

const getAbsenceConfig = (
	theme: Theme,
	mode: ThemeMode,
	absenceType: AbsenceType
): EventConfig => {
	const styles = iconStyles(theme);
	const fallback = noIconStyles(theme);
	const config = styles[absenceType] || null;

	const Icon = config?.icon ?? fallback.icon;
	const color = config?.color?.[mode] ?? fallback.color[mode];
	const background = config?.iconBackground?.[mode] ?? '#F2F2F2';

	return {
		icon: <Icon />,
		color,
		background,
	};
};

export const getEventConfig = (
	theme: Theme,
	mode: ThemeMode,
	event: UpcomingEvent
): EventConfig => {
	const { type, inquiryType } = event;

	if (type === UpcomingEventType.Absence && inquiryType) {
		return getAbsenceConfig(theme, mode, inquiryType);
	}

	return upcomingEventsOptions[type];
};
