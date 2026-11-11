import { useTheme } from '@emotion/react';
import { SearchWithCrossIcon } from '@mms/mms-ui-library/icons/base/SearchWithCrossIcon';

import { NoDataFound } from '@/components/NoDataFoundWrapper/NoDataFound';
import { NoDataWrapperProps } from '@/components/NoDataFoundWrapper/types';

export function NoDataFoundWrapper({
	children,
	isEmptyData,
	icon,
	title,
	controls,
}: NoDataWrapperProps) {
	const theme = useTheme();
	const basicIcon = icon || (
		<SearchWithCrossIcon color={theme.palette['graphics-primary-2']} />
	);

	return isEmptyData ? (
		<NoDataFound icon={basicIcon} {...{ controls, title }} />
	) : (
		children
	);
}
