import { useQueryClient } from '@tanstack/react-query';
import { useCallback, useMemo, useRef } from 'react';

import { AbsencesCarousel } from '../AbsencesCarousel';
import { UserAbsenceItem } from '../UserAbsenceItem';
import { MIN_ITEM_WIDHT } from '../UserAbsenceItem/constants';
import { ModalData } from '../UserAbsenceItem/types';

import {
	NotificationType,
	REQUEST_ABSENCE_CREATED,
} from '@/constants/notifications';
import { absencesQueriesKeys, homeQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { RequestAbsenceForm } from '@/forms/Absence/RequestAbsenceForm';
import { RequestDataValues } from '@/forms/Absence/RequestAbsenceForm/types';
import { convertFormDataToSubmit } from '@/forms/Absence/RequestAbsenceForm/utils/convertFormDataToSubmit';
import { checkIsDocumentRequest } from '@/helpers/absences';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useResize } from '@/hooks/useResize';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useCreateRequest } from '@/queries/inquiries/useCreateRequest';

import { GAP_BETWEEN_ELEMENTS } from './constants';
import { AbsenceListWrapper } from './styles';
import { AbsenceItemListProps } from './types';

export function AbsenceItemList({
	data,
	userId,
	locationId,
	accessLevel,
}: AbsenceItemListProps) {
	const toast = useToast();
	const queryClient = useQueryClient();

	const [windowWidth] = useResize();

	const carouselRef = useRef<HTMLDivElement>(null);

	const {
		handleOpen,
		isOpen,
		handleClose,
		id: modalData,
	} = useShowModal<ModalData>();
	const { mutateAsync: createRequest, isPending: isRequestCreating } =
		useCreateRequest();

	const handleOpenModal = useCallback(
		(params: ModalData) => {
			handleOpen(params);
		},
		[handleOpen]
	);

	const sliderWidth = carouselRef.current?.offsetWidth as number;

	const slidesToShow = useMemo(
		() => sliderWidth / (MIN_ITEM_WIDHT + GAP_BETWEEN_ELEMENTS),
		[sliderWidth, windowWidth]
	);

	const isAllSlidesVisible = slidesToShow > (data?.length as number);

	const absencesList = useMemo(
		() =>
			data
				?.filter(({ type }) => !checkIsDocumentRequest(type))
				.map((item) => (
					<UserAbsenceItem
						key={item.inquiryTemplateId}
						absenceItem={item}
						{...{ handleOpen: handleOpenModal, userId, accessLevel }}
					/>
				)),
		[data, accessLevel, userId, handleOpen]
	);

	const handleSubmitRequest = (values: RequestDataValues) =>
		createRequest(convertFormDataToSubmit({ ...values, userId }), {
			onError: createOnErrorHandler(toast),
			onSuccess: () => {
				toast.open(
					createToast(NotificationType.SUCCESS, REQUEST_ABSENCE_CREATED)
				);

				queryClient.invalidateQueries({
					queryKey: [absencesQueriesKeys.getUserAbsencesRequest],
				});
				queryClient.invalidateQueries({
					queryKey: [absencesQueriesKeys.getUserTransactionsHistory],
				});
				queryClient.invalidateQueries({
					queryKey: [absencesQueriesKeys.getUserAbsences],
				});
				queryClient.invalidateQueries({
					queryKey: [homeQueriesKeys.getUserUpcomingEvents],
				});

				handleClose();
			},
		});

	return (
		<AbsenceListWrapper ref={carouselRef}>
			{isAllSlidesVisible ? (
				absencesList
			) : (
				<AbsencesCarousel sliderWidth={sliderWidth} slidesToShow={slidesToShow}>
					{absencesList}
				</AbsencesCarousel>
			)}

			{isOpen && modalData && (
				<RequestAbsenceForm
					handleClose={handleClose}
					handleSubmit={handleSubmitRequest}
					initialAbsenceTypeId={modalData.templateId}
					userId={userId}
					formType={modalData.formType}
					{...{ locationId, isRequestCreating }}
				/>
			)}
		</AbsenceListWrapper>
	);
}
