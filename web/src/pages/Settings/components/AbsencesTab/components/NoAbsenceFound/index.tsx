import { useTheme } from '@emotion/react';
import { VacationIcon } from '@mms/mms-ui-library/icons/base/VacationIcon';
import { PropsWithChildren } from 'react';

import { ButtonAdd } from '@/components/ButtonAdd';
import { NoDataFoundWrapper } from '@/components/NoDataFoundWrapper';
import { IconSize } from '@/constants/iconSizes';
import { ControlWrapper } from '@/pages/Settings/components/AbsencesTab/components/NoAbsenceFound/styles';
import { NoAbsenceFoundProps } from '@/pages/Settings/components/AbsencesTab/components/NoAbsenceFound/types';

import {
	ADD_ABSENCE_BUTTON_TITLE,
	NO_ABSENCE_DATA_FOUND_TITLE,
} from './constants';

export function NoAbsenceFound({
	handleClickControl,
	isEmptyData,
	children,
}: PropsWithChildren<NoAbsenceFoundProps>) {
	const theme = useTheme();

	return (
		<NoDataFoundWrapper
			{...{ isEmptyData }}
			title={NO_ABSENCE_DATA_FOUND_TITLE}
			icon={
				<VacationIcon
					width={IconSize.XL}
					height={IconSize.XL}
					color={theme.palette['graphics-primary-2']}
				/>
			}
			controls={
				<ControlWrapper>
					<ButtonAdd
						onAdd={handleClickControl}
						title={ADD_ABSENCE_BUTTON_TITLE}
					/>
				</ControlWrapper>
			}
		>
			{children}
		</NoDataFoundWrapper>
	);
}
