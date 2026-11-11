import { Theme } from '@emotion/react';
import { HomeIcon } from '@mms/mms-ui-library/icons/base/HomeIcon';
import { MilitaryIcon } from '@mms/mms-ui-library/icons/base/MilitaryIcon';
import { OwnExpenseIcon } from '@mms/mms-ui-library/icons/base/OwnExpenseIcon';
import { SickDayIcon } from '@mms/mms-ui-library/icons/base/SickDayIcon';
import { SickLeaveIcon } from '@mms/mms-ui-library/icons/base/SickLeaveIcon';
import { SlashIcon } from '@mms/mms-ui-library/icons/base/SlashIcon';
import { SocialVacationIcon } from '@mms/mms-ui-library/icons/base/SocialVacationIcon';
import { VacationIcon } from '@mms/mms-ui-library/icons/base/VacationIcon';
import { WeekendTransferIcon } from '@mms/mms-ui-library/icons/base/WeekendTransferIcon';
import { lightTheme, WHITE } from '@mms/mms-ui-library/theme';

import { ThemeMode } from '../theme';

import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { fromToDateObject } from '@/constants/defaultValues';
import { ALL_ID } from '@/constants/filters';
import { convertConstantToSelectionModalOption } from '@/helpers/convertToSelectionModalOptions';
import { createGradient } from '@/helpers/createGradient';
import { DocumentIcon } from '@/layouts/components/ServiceRequestPopup/components/DocumentIcon';
import {
	AbsenceRequestFilters,
	AbsenceRequestStatus,
	AbsenceTransactionHistoryType,
	AbsenceType,
	AccrualType,
	TransferType,
} from '@/types/absences';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

const { DARK, LIGHT } = ThemeMode;
const {
	Vacation,
	SocialVacation,
	MilitaryEnlistmentOffice,
	HomeOffice,
	SickDay,
	SickLeave,
	WeekendTransfer,
	OwnExpense,
	OtherType,
	Documents,
} = AbsenceType;

const { ProportianallyEachMonth, OnceInYear } = AccrualType;

const { Transferred, Reset } = TransferType;

const { Accrual, Using, Disposal } = AbsenceTransactionHistoryType;

const { Open, Approved, Rejected, Canceled } = AbsenceRequestStatus;

export const absencesType: Record<AbsenceType, string> = {
	[Vacation]: 'Vacation',
	[SickLeave]: 'Sick leave',
	[HomeOffice]: 'Home office',
	[OwnExpense]: 'Own expense',
	[SickDay]: 'Sick day',
	[WeekendTransfer]: 'Weekend transfer',
	[MilitaryEnlistmentOffice]: 'Military enlistment office',
	[SocialVacation]: 'Social vacation',
	[Documents]: 'Documents',
	[OtherType]: 'Other type',
};

export const accrualsType: Record<AccrualType, string> = {
	[ProportianallyEachMonth]: 'Proportionally every month',
	[OnceInYear]: 'Proportionally every year',
};

export const transfersType: Record<TransferType, string> = {
	[Transferred]: 'Transferred',
	[Reset]: 'Reset',
};

export const activitiesType: Record<string, string> = {
	NonWorkingPaid: 'Non-working, paid',
	WorkingPaid: 'Working, paid',
	NonWorkingUnpaid: 'Non-working, unpaid',
};

export const trackingUnitsType: Record<string, string> = {
	Days: 'days',
	Hours: 'hours',
};

export const ABSENCE_ENTITY_NAME = 'Absence';
export const REQUEST_FOR_LABEL = 'request for';

const WHITE_OPACITY_IN_COLOR = 0.85;

const getHomeBackgroundColor = (theme: Theme) =>
	createGradient(
		theme.palette['graphics-accent'],
		WHITE,
		WHITE_OPACITY_IN_COLOR
	);

export const iconStyles = (theme: Theme) => ({
	[Vacation]: {
		icon: VacationIcon,
		color: {
			[LIGHT]: lightTheme.palette['text-turquoise'],
			[DARK]: lightTheme.palette['text-turquoise'],
		},
		iconBackground: {
			[LIGHT]: lightTheme.palette['background-turquoise'],
			[DARK]: lightTheme.palette['background-turquoise'],
		},
		background: {
			[LIGHT]: '#DFF7F599',
			[DARK]: '#78F2E840',
		},
	},
	[SickLeave]: {
		icon: SickLeaveIcon,
		color: {
			[LIGHT]: lightTheme.palette['text-error'],
			[DARK]: lightTheme.palette['text-error'],
		},
		iconBackground: {
			[LIGHT]: lightTheme.palette['background-red'],
			[DARK]: lightTheme.palette['background-red'],
		},
		background: {
			[LIGHT]: '#FAE2DF99',
			[DARK]: '#ED7D6F40',
		},
	},
	[HomeOffice]: {
		icon: HomeIcon,
		color: {
			[LIGHT]: lightTheme.palette['graphics-accent'],
			[DARK]: lightTheme.palette['graphics-accent'],
		},
		iconBackground: {
			[LIGHT]: getHomeBackgroundColor(theme),
			[DARK]: getHomeBackgroundColor(theme),
		},
		background: {
			[LIGHT]: '#FDEBD999',
			[DARK]: '#FFBF7F40',
		},
	},
	[OwnExpense]: {
		icon: OwnExpenseIcon,
		color: {
			[LIGHT]: lightTheme.palette['text-violet'],
			[DARK]: lightTheme.palette['text-violet'],
		},
		iconBackground: {
			[LIGHT]: lightTheme.palette['background-purple'],
			[DARK]: lightTheme.palette['background-purple'],
		},
		background: {
			[LIGHT]: '#EFEAFF99',
			[DARK]: '#B49DFF40',
		},
	},
	[SickDay]: {
		icon: SickDayIcon,
		color: {
			[LIGHT]: lightTheme.palette['text-pink'],
			[DARK]: lightTheme.palette['text-pink'],
		},
		iconBackground: {
			[LIGHT]: lightTheme.palette['background-pink'],
			[DARK]: lightTheme.palette['background-pink'],
		},
		background: {
			[LIGHT]: '#FFE1F499',
			[DARK]: '#FE8CD440',
		},
	},
	[WeekendTransfer]: {
		icon: WeekendTransferIcon,
		color: {
			[LIGHT]: lightTheme.palette['feedback-success'],
			[DARK]: lightTheme.palette['feedback-success'],
		},
		iconBackground: {
			[LIGHT]: lightTheme.palette['background-green'],
			[DARK]: lightTheme.palette['background-green'],
		},
		background: {
			[LIGHT]: '#C7FFCC99',
			[DARK]: '#96FFA040',
		},
	},
	[MilitaryEnlistmentOffice]: {
		icon: MilitaryIcon,
		color: {
			[LIGHT]: lightTheme.palette['text-light-green'],
			[DARK]: lightTheme.palette['text-light-green'],
		},
		iconBackground: {
			[LIGHT]: lightTheme.palette['background-light-green'],
			[DARK]: lightTheme.palette['background-light-green'],
		},
		background: {
			[LIGHT]: '#E2FFA999',
			[DARK]: '#D5FF8140',
		},
	},
	[SocialVacation]: {
		icon: SocialVacationIcon,
		color: {
			[LIGHT]: lightTheme.palette['text-blue'],
			[DARK]: lightTheme.palette['text-blue'],
		},
		iconBackground: {
			[LIGHT]: lightTheme.palette['background-blue'],
			[DARK]: lightTheme.palette['background-blue'],
		},
		background: {
			[LIGHT]: '#D2EAFF99',
			[DARK]: '#9DD1FF40',
		},
	},
	[OtherType]: {
		icon: SlashIcon,
		color: {
			[LIGHT]: lightTheme.palette['graphics-light-gray'],
			[DARK]: lightTheme.palette['graphics-light-gray'],
		},
		iconBackground: {
			[LIGHT]: lightTheme.palette['background-primary-1'],
			[DARK]: lightTheme.palette['background-primary-1'],
		},
		background: {
			[LIGHT]: '#F2F2F2B2',
			[DARK]: '#F2F2F233',
		},
	},
	[Documents]: {
		icon: DocumentIcon,
		color: {
			[LIGHT]: theme.palette['graphics-primary-2'],
			[DARK]: theme.palette['graphics-primary-2'],
		},
		iconBackground: {
			[LIGHT]: '#E9EAEF',
			[DARK]: '#E9EAEF',
		},
		background: {
			[LIGHT]: '#E9EAEFB3',
			[DARK]: '#E9EAEF33',
		},
	},
});

export const noIconStyles = (theme: Theme) => ({
	icon: SlashIcon,
	color: {
		[LIGHT]: theme.palette['graphics-light-gray'],
		[DARK]: theme.palette['graphics-light-gray'],
	},
});

export const noTimelineIconStyles = (theme: Theme) => ({
	icon: SlashIcon,
	color: theme.palette['text-gray'],
	background: theme.palette['background-primary-1'],
});

export const timelineIconStyles = (theme: Theme) => ({
	[Vacation]: {
		icon: VacationIcon,
		color: theme.palette['text-turquoise'],
		background: theme.palette['background-turquoise'],
	},
	[SickLeave]: {
		icon: SickLeaveIcon,
		color: theme.palette['text-red'],
		background: theme.palette['background-red'],
	},
	[HomeOffice]: {
		icon: HomeIcon,
		color: theme.palette['text-orange'],
		background: theme.palette['background-orange'],
	},
	[OwnExpense]: {
		icon: OwnExpenseIcon,
		color: theme.palette['text-purple'],
		background: theme.palette['background-purple'],
	},
	[SickDay]: {
		icon: SickDayIcon,
		color: theme.palette['text-pink'],
		background: theme.palette['background-pink'],
	},
	[WeekendTransfer]: {
		icon: WeekendTransferIcon,
		color: theme.palette['text-green'],
		background: theme.palette['background-green'],
	},
	[MilitaryEnlistmentOffice]: {
		icon: MilitaryIcon,
		color: theme.palette['text-light-green'],
		background: theme.palette['background-light-green'],
	},
	[SocialVacation]: {
		icon: SocialVacationIcon,
		color: theme.palette['text-blue'],
		background: theme.palette['background-blue'],
	},
	[OtherType]: noTimelineIconStyles(theme),
});

export const DEFAULT_ABSENCE_REQUEST_EDIT_COLUMN_INDEX = 7;

export const MIN_ABSENCE_CARDS_HEIGHT = 166;
export const MIN_ABSENCE_REQUESTS_HEIGHT = 254;
export const MIN_ABSENCE_HISTORY_HEIGHT = 214;
export const MOBILE_MIN_ABSENCE_HISTORY_HEIGHT = 368;

export const absenceTransactionDescriptionKey = {
	[Accrual]: 'proportionalAccrual',
	[Using]: 'request',
	[Disposal]: 'resetAnnualBalance',
} as const;

export const initialAbsenceFilterValue: SimpleConfigurableConfigurationItem = {
	id: ALL_ID,
	name: 'All absences',
};

export const initialYearFilterValue: SelectionModalOption = {
	id: ALL_ID,
	description: 'All years',
};

export const orderValuesForAbsenceRequest = {
	orderWeight: 'statusOrderWeight',
	dateFrom: 'dateFrom',
};

export const INQUIRY_TYPE_ORDER_WEIGHT = 'inquiryTypeOrderWeight';

export const defaultAbsenceRequestFilterValues: AbsenceRequestFilters = {
	dateTo: fromToDateObject,
	dateFrom: fromToDateObject,
	inquiryTemplateName: [],
	status: [],
};

export const absenceRequestColumns = {
	inquiryTemplateName: {
		key: 'inquiryTemplateName',
		sortable: true,
	},
	dateFrom: {
		key: 'dateFrom',
		sortable: true,
	},
	dateTo: {
		key: 'dateTo',
		sortable: true,
	},
	quantity: {
		key: 'quantity',
		sortable: false,
	},
	status: {
		key: 'status',
		sortable: true,
	},
};

export const PM_ROLE = 'PM';

export const ABSENCE_AVAILABLE = 'Available';
export const ABSENCE_BALANCE = 'Balance';
export const ABSENCE_REMAINING = 'Remaining';
export const ABSENCE_SELECTED = 'Selected';

export const requestStatusOptions: Array<SelectionModalOption> = [
	convertConstantToSelectionModalOption(Open),
	convertConstantToSelectionModalOption(Rejected),
	convertConstantToSelectionModalOption(Approved),
	convertConstantToSelectionModalOption(Canceled),
];
