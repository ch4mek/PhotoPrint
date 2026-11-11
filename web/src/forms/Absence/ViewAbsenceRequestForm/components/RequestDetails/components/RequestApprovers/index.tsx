import { spaces } from '@mms/mms-ui-library/theme';

import { ApproverItem } from '../ApproverItem';

import { FormSection } from '@/components/FormSection';
import { getApproverData } from '@/helpers/absences';
import { UserInquiryStatusByApprover } from '@/types/absences';

import { SECTION_TITLE } from './constants';
import { ApproversList } from './styles';
import { RequestApproversProps } from './types';

export function RequestApprovers({ data, inquiryId }: RequestApproversProps) {
	return (
		<FormSection title={SECTION_TITLE} gap={spaces.s}>
			<ApproversList>
				{data.map(
					({
						id,
						status,
						acceptUser,
						acceptedByUser,
						rejectedByUser,
						acceptUserPosition,
						acceptPolicyScope,
					}) => {
						const { approver, isApproverNotAccepter } = getApproverData(
							status,
							acceptUser,
							acceptedByUser,
							rejectedByUser
						);

						const isDisabled =
							status === UserInquiryStatusByApprover.Cancelled ||
							isApproverNotAccepter;

						return (
							<ApproverItem
								key={id}
								{...{
									isDisabled,
									status,
									acceptUserPosition,
									acceptPolicyScope,
									inquiryId,
								}}
								inquiryAcceptId={id}
								item={acceptUser}
								approver={isApproverNotAccepter ? approver : undefined}
							/>
						);
					}
				)}
			</ApproversList>
		</FormSection>
	);
}
