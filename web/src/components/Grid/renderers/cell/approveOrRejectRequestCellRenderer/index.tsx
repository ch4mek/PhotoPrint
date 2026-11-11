import { Button } from '@mms/mms-ui-library/components/Button';
import {
	PickerButton,
	PickerButtonOption,
} from '@mms/mms-ui-library/components/PickerButton';
import { OptionDotsIcon } from '@mms/mms-ui-library/icons/base/OptionDotsIcon';
import { useMemo, useRef } from 'react';
import { RenderCellProps } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import { useAuth } from '@/context/AuthContext';
import { RequestManageForm } from '@/forms/Absence/RequestManageForm';
import { checkIsDocumentRequest } from '@/helpers/absences';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { useAddScrollListener } from '@/hooks/useAddScrollListener';
import { useShowModal } from '@/pages/hooks/useShowModal';
import {
	ManageRequestType,
	AbsenceRequestGridRow,
	AbsenceRequestStatus,
} from '@/types/absences';
import { UserClaims } from '@/types/claims';

const { Accept, Reject } = ManageRequestType;

export function ApproveOrRejectRequestCellRenderer({
	row,
}: RenderCellProps<AbsenceRequestGridRow>) {
	const {
		isOpen,
		handleClose,
		handleOpen,
		id: itemRequest,
	} = useShowModal<ManageRequestType>();
	const { id: userId, claims } = useAuth();
	const { t } = useTranslation('common', { keyPrefix: 'controlButtons' });

	const {
		status,
		createdForUser,
		inquiryTemplateName,
		id: requestId,
		inquiryType,
	} = row;
	const { requestStatus, canCurrentUserApprove, canCurrentUserReject } = status;

	const pickerButtonRef = useRef<HTMLButtonElement>(null);
	const [isOpenPopover, setIsOpenPopover] =
		useAddScrollListener(pickerButtonRef);

	const handleManageRequest = (type: ManageRequestType) => () => {
		handleOpen(type);
	};

	const isAllowToApproveRequest =
		requestStatus === AbsenceRequestStatus.Open && canCurrentUserApprove;

	const isForbiddenToRejectOwnRequest =
		createdForUser.id === userId &&
		!checkIsHasClaims([UserClaims.ManageAllRequests], claims);

	const isRejectableByStatus =
		requestStatus === AbsenceRequestStatus.Open ||
		(requestStatus === AbsenceRequestStatus.Approved &&
			!isForbiddenToRejectOwnRequest);

	const isAllowToRejectRequest = isRejectableByStatus && canCurrentUserReject;

	const options = useMemo<PickerButtonOption[]>(() => {
		const buttons: PickerButtonOption[] = [];

		if (isAllowToApproveRequest) {
			buttons.push({
				value: 'approve',
				addon: (
					<Button onClick={handleManageRequest(Accept)}>
						{checkIsDocumentRequest(inquiryType) ? t('done') : t('approve')}
					</Button>
				),
			});
		}

		if (isAllowToRejectRequest) {
			buttons.push({
				value: 'reject',
				addon: (
					<Button color="secondary" onClick={handleManageRequest(Reject)}>
						{t('reject')}
					</Button>
				),
			});
		}

		return buttons;
	}, [
		inquiryType,
		canCurrentUserApprove,
		canCurrentUserReject,
		handleManageRequest,
	]);

	return (
		<>
			{options.length > 0 && (
				<PickerButton
					icon={<OptionDotsIcon />}
					popoverPosition="left-start"
					displayOnlyAddon
					options={options}
					isOpenPopover={isOpenPopover}
					setIsOpenPopover={setIsOpenPopover}
					ref={pickerButtonRef}
					removeOptionPaddings
				/>
			)}
			{isOpen && itemRequest && (
				<RequestManageForm
					item={{
						requestName: inquiryTemplateName,
						id: requestId,
						createdForUser,
					}}
					type={itemRequest}
					handleClose={handleClose}
				/>
			)}
		</>
	);
}
