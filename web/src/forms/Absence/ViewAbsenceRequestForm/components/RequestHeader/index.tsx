import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';

import { RequestControls } from '../RequestControls';
import { RequestHeaderInformation } from '../RequestHeaderInformation';

import { RequestManageForm } from '@/forms/Absence/RequestManageForm';
import { RequestStatus } from '@/forms/Absence/ViewAbsenceRequestForm/components/RequestStatus';
import { MOBILE_VIEW_REQUEST_FORM_BREAKPOINT } from '@/forms/Absence/ViewAbsenceRequestForm/constants';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { AbsenceRequestStatus, ManageRequestType } from '@/types/absences';

import { ControlsWrapper, RequestHeaderWrapper } from './styles';
import { RequestHeaderProps } from './types';

export function RequestHeader({ item }: RequestHeaderProps) {
	const isMobileView = !useHasScreenWidth(MOBILE_VIEW_REQUEST_FORM_BREAKPOINT);
	const {
		isOpen,
		handleClose,
		handleOpen,
		id: itemRequest,
	} = useShowModal<ManageRequestType>();

	if (!item) {
		return;
	}

	const {
		canCurrentUserApprove,
		canCurrentUserReject,
		id: requestId,
		inquiryTemplateName,
		createdForUser,
		inquiryType,
		status,
	} = item;

	const isAllowToApproveRequest =
		status === AbsenceRequestStatus.Open && canCurrentUserApprove;

	const isAllowToRejectRequest =
		(status === AbsenceRequestStatus.Open ||
			status === AbsenceRequestStatus.Approved) &&
		canCurrentUserReject;

	return (
		<RequestHeaderWrapper>
			<RequestHeaderInformation {...{ item }} withStatus={!isMobileView} />
			<ControlsWrapper>
				{isMobileView && <RequestStatus status={item.status} />}
				{(isAllowToApproveRequest || isAllowToRejectRequest) && (
					<RequestControls
						{...{ inquiryType, handleOpen }}
						canCurrentUserApprove={isAllowToApproveRequest}
						canCurrentUserReject={isAllowToRejectRequest}
					/>
				)}
			</ControlsWrapper>
			{isOpen && itemRequest && (
				<RequestManageForm
					type={itemRequest}
					handleClose={handleClose}
					item={{
						requestName: inquiryTemplateName,
						id: requestId,
						createdForUser,
					}}
				/>
			)}
		</RequestHeaderWrapper>
	);
}
