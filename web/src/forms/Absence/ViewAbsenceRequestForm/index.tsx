import { useTheme } from '@emotion/react';
import { LinearLoader } from '@mms/mms-ui-library/components/LinearLoader';
import { LoaderVariant } from '@mms/mms-ui-library/components/LinearLoader/types';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useMemo } from 'react';

import {
	NotificationType,
	ABSENCE_REQUEST_NOT_FOUND,
} from '@/constants/notifications';
import { useToast } from '@/context/Toast';
import {
	ABSENCE_REQUEST_ENTITY,
	gridAreas,
} from '@/forms/Absence/ViewAbsenceRequestForm/constants';
import { ViewAbsenceRequestFormProps } from '@/forms/Absence/ViewAbsenceRequestForm/types';
import { FormWithSaveExit } from '@/forms/components/FormWithSaveExit';
import { convertDataToHolidays } from '@/helpers/absences';
import { createToast } from '@/helpers/createToast';
import { useResize } from '@/hooks/useResize';
import { LoaderWrapper } from '@/layouts/main/styles';
import { useGetHolidaysByUserCountry } from '@/queries/calendar/getHolidaysByUserCountry';
import { useGetInquiryRequestById } from '@/queries/inquiries/useGetInquiryRequestById';

import { RequestDetails } from './components/RequestDetails';
import { RequestHeader } from './components/RequestHeader';
import { RequestParticipants } from './components/RequestParticipants';
import {
	AbsenceRequestChangesProvider,
	useAbsenceRequestChanges,
} from './context/AbsenceRequestChangesContext';
import {
	FormWrapper,
	getPopUpStylesConfig,
	RequestContent,
	SectionWrapper,
} from './styles';

const { participants, details } = gridAreas;

function ViewAbsenceRequestFormInner({
	id,
	handleClose,
}: ViewAbsenceRequestFormProps) {
	const theme = useTheme();
	const toast = useToast();
	const [, windowHeight] = useResize();
	const { hasUnsavedChanges } = useAbsenceRequestChanges();

	const { data: item, isFetching } = useGetInquiryRequestById(id, {
		enabled: !!id,
		onError: () => {
			toast.open(
				createToast(NotificationType.ERROR, ABSENCE_REQUEST_NOT_FOUND)
			);
			handleClose();
		},
	});

	const { data, isLoading: isLoadingHolidays } = useGetHolidaysByUserCountry(
		item?.countryId || null,
		{
			enabled: !!item?.createdForUser?.registeredLocationId,
		}
	);

	const holidays = useMemo(() => convertDataToHolidays(data), [data]);

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, windowHeight),
		[theme, windowHeight]
	);

	if (!item) {
		return (
			<LoaderWrapper>
				<LinearLoader variant={LoaderVariant.Indeterminate} />
			</LoaderWrapper>
		);
	}

	return (
		<FormWithSaveExit isFormDirty={hasUnsavedChanges} onClose={handleClose}>
			{(handleOutsideClose) => (
				<PopUp
					type="form"
					title={`${item.inquiryTemplateName} ${ABSENCE_REQUEST_ENTITY}`}
					onClose={handleClose}
					onShadowClick={handleOutsideClose}
					stylesConfig={popUpStylesConfig}
					showLoader={isLoadingHolidays || isFetching}
				>
					<FormWrapper>
						<RequestHeader {...{ item }} />

						<RequestContent>
							<SectionWrapper gridArea={details}>
								<RequestDetails {...{ item, holidays }} />
							</SectionWrapper>
							<SectionWrapper gridArea={participants}>
								<RequestParticipants
									managerData={item?.managerOfUser}
									createdData={item?.createdByUser}
									requestFromData={item?.createdForUser}
								/>
							</SectionWrapper>
						</RequestContent>
					</FormWrapper>
				</PopUp>
			)}
		</FormWithSaveExit>
	);
}

export function ViewAbsenceRequestForm(props: ViewAbsenceRequestFormProps) {
	return (
		<AbsenceRequestChangesProvider>
			<ViewAbsenceRequestFormInner {...props} />
		</AbsenceRequestChangesProvider>
	);
}
