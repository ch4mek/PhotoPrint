import { TextArea } from '@mms/mms-ui-library/components/TextArea';

import { CountValueItem } from '@/components/CountValueItem';
import { ABSENCE_SELECTED } from '@/constants/absence';
import { MIN_COUNT_OF_ROWS } from '@/constants/commonStyles';
import {
	checkIsDocumentRequest,
	getCountLabelWithUnit,
} from '@/helpers/absences';
import { TrackingUnitType } from '@/types/absences';

import { FilesGroup } from './components/FilesGroup';
import { RequestApprovers } from './components/RequestApprovers';
import { RequestCalendar } from './components/RequestCalendar';
import { RequestComments } from './components/RequestComments';
import {
	CalendarInfoWrapper,
	CountValuesWrapper,
	RequestDetailsWrapper,
} from './styled';
import { RequestDetailsProps } from './types';
import { getBalanceInfo } from './utils/getBalanceInfo';

export function RequestDetails({ item, holidays }: RequestDetailsProps) {
	const handleChangeTextArea = () => {};

	if (!item) {
		return null;
	}

	const {
		id,
		notes,
		userInquiryFiles,
		userInquiryAccepts,
		dateTo,
		dateFrom,
		totalAccountantDays,
		totalHours,
		remainingDays,
		balanceDays,
		balanceHours,
		inquiryType,
	} = item;

	const balanceInfo = getBalanceInfo(remainingDays, balanceDays, balanceHours);

	const isDaysTrackingType = totalAccountantDays != null;
	const trackingType = isDaysTrackingType
		? TrackingUnitType.Days
		: TrackingUnitType.Hours;
	const selectedLabel = getCountLabelWithUnit(trackingType, ABSENCE_SELECTED);
	const selectedCount = isDaysTrackingType ? totalAccountantDays : totalHours;

	return (
		<RequestDetailsWrapper>
			{!checkIsDocumentRequest(inquiryType) && (
				<CalendarInfoWrapper>
					<CountValuesWrapper>
						<CountValueItem title={selectedLabel} count={selectedCount ?? 0} />
						<CountValueItem
							title={balanceInfo.label}
							count={balanceInfo.count}
						/>
					</CountValuesWrapper>

					<RequestCalendar
						{...{ totalHours, totalAccountantDays, dateTo, dateFrom, holidays }}
					/>
				</CalendarInfoWrapper>
			)}

			<TextArea
				label="Notes"
				size="fullWidth"
				minRows={MIN_COUNT_OF_ROWS}
				value={notes || ''}
				disabled
				onChange={handleChangeTextArea}
			/>
			{!!userInquiryFiles.length && <FilesGroup data={userInquiryFiles} />}
			{!!userInquiryAccepts.length && (
				<RequestApprovers data={userInquiryAccepts} inquiryId={id} />
			)}
			<RequestComments requestId={id} />
		</RequestDetailsWrapper>
	);
}
