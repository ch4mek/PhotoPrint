import { useTheme } from '@emotion/react';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { RenderCellProps } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import { Tag } from '@/components/Tag';
import { getApproverData } from '@/helpers/absences';
import { getRequestStatusColor } from '@/helpers/getRequestStatusColor';
import { UserInquiryStatusByApprover } from '@/types/absences';

import { STATUS_TAG_WIDTH } from './constants';
import { IconWrapper, StatusCellWrapper, StatusesWrapper } from './styles';
import { RequestStatusCellRendererOptions, StatusCellProps } from './types';
import { getApproversRequestStatus } from './utils/getApproversRequestStatus';
import { getTooltipText } from './utils/getTooltipText';

const { Cancelled } = UserInquiryStatusByApprover;

export function RequestStatusCellRenderer<T extends StatusCellProps>(
	{ row }: RenderCellProps<T, unknown>,
	{ onCellClick, isClickable = false }: RequestStatusCellRendererOptions<T> = {}
) {
	const theme = useTheme();
	const { t } = useTranslation('common');
	const {
		status: { requestStatus, approvals },
	} = row;

	const backgroundColor = getRequestStatusColor(requestStatus);

	const handleClick = () => {
		if (!row.disabled && onCellClick && isClickable) {
			onCellClick(row);
		}
	};

	const statusTag = (
		<Tag
			{...{ backgroundColor, isClickable }}
			width={STATUS_TAG_WIDTH}
			onClick={handleClick}
		>
			{requestStatus}
		</Tag>
	);

	if (!approvals) {
		return statusTag;
	}

	return (
		<StatusCellWrapper>
			{statusTag}
			{approvals.length > 0 && (
				<StatusesWrapper>
					{approvals.map(
						({ acceptUser, status, id, rejectedByUser, acceptedByUser }) => {
							const isCancelled = status === Cancelled;
							const { approver, isApproverNotAccepter } = getApproverData(
								status,
								acceptUser,
								acceptedByUser,
								rejectedByUser
							);

							const { icon, color } = getApproversRequestStatus(theme)[status];

							return (
								<Tooltip
									key={id}
									text={getTooltipText(status, approver, t)}
									position="top"
								>
									<IconWrapper
										{...{ color }}
										isDisabled={isCancelled || isApproverNotAccepter}
									>
										{icon}
									</IconWrapper>
								</Tooltip>
							);
						}
					)}
				</StatusesWrapper>
			)}
		</StatusCellWrapper>
	);
}
