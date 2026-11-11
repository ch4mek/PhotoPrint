import { Theme } from '@emotion/react';

import { iconStyles, noIconStyles } from '@/constants/absence';
import { ThemeMode } from '@/theme';
import { AbsenceType } from '@/types/absences';

export const getAbsenceConfig = (
	theme: Theme,
	mode: ThemeMode,
	absenceType: AbsenceType | null
) => {
	const styles = iconStyles(theme);
	const fallback = noIconStyles(theme);
	const config = absenceType ? styles[absenceType] : null;

	const Icon = config?.icon ?? fallback.icon;
	const color = config?.color?.[mode] ?? fallback.color[mode];
	const background = config?.iconBackground?.[mode] ?? 'transparent';

	return { Icon, color, background };
};
