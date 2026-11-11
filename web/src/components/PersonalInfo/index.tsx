import { Switch } from '@mms/mms-ui-library/components/Switch';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { useTheme } from '@mui/material';
import { useQueryClient } from '@tanstack/react-query';
import { ChangeEvent, useCallback, useMemo } from 'react';

import { Loader } from '../Loader';

import {
	NEED_PROJECT_UPDATED,
	NotificationType,
} from '@/constants/notifications';
import {
	payloadQueriesKeys,
	TeamboardQueriesKeys,
} from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { NEED_A_PROJECT_TITLE } from '@/forms/PayloadForm/components/PayloadDetails/constants';
import { NeedProjectContainer } from '@/forms/PayloadForm/components/PayloadDetails/styles';
import { createToast } from '@/helpers/createToast';
import { getSeniorityAndSpecialization } from '@/helpers/getSeniorityAndSpecialization';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useUpdateNeedProject } from '@/queries/teamboard/useUpdateNeedProject';

import { CommentContainer } from './components/CommentContainer';
import {
	CommentSection,
	EmailTypography,
	FixedTypography,
	InfoWrapper,
	NameAndSeniorityWrapper,
	NameWrapper,
	NeedAProjectTypography,
	PersonalInfoHeaderActionsWrapper,
	PersonalInfoTopRow,
	PersonalInfoWrapper,
	SpecializationName,
	Wrapper,
} from './styles';
import { PersonalInfoProps } from './types';

export function PersonalInfo({
	isShowLoader,
	seniority,
	primarySpecialization,
	fullName,
	isShowComments,
	AvatarComponent,
	avatarProps,
	comments,
	refetch,
	position,
	userId,
	email,
	TeamsChatButtonComponent,
	needProject = false,
	isShowSpecialization = true,
	isNeedWrapperStyles = true,
	withNeedAProjectButton = false,
}: PersonalInfoProps) {
	const specializationAndSeniorityValue = useMemo(
		() => getSeniorityAndSpecialization(seniority, primarySpecialization),
		[seniority, primarySpecialization]
	);
	const theme = useTheme();
	const toast = useToast();
	const queryClient = useQueryClient();

	const updateCommentMutation = useUpdateNeedProject({
		onSuccess: () => {
			toast.open(createToast(NotificationType.SUCCESS, NEED_PROJECT_UPDATED));

			queryClient.invalidateQueries({
				queryKey: [TeamboardQueriesKeys.personalData],
			});
			queryClient.invalidateQueries({
				queryKey: [payloadQueriesKeys.getPayloadData],
			});
			refetch();
		},
		onError: createOnErrorHandler(toast),
	});

	const handleToggleChange = useCallback(
		(_: ChangeEvent<HTMLInputElement>, { checked }: { checked: boolean }) => {
			updateCommentMutation
				.mutateAsync({ needProject: checked, userId: Number(userId) })
				.then(() => refetch());
		},
		[]
	);

	return (
		<Wrapper isNeedWrapperStyles={isNeedWrapperStyles}>
			<Loader isLoading={isShowLoader} size="small">
				<InfoWrapper>
					<AvatarComponent {...avatarProps} />
					<PersonalInfoWrapper>
						<PersonalInfoTopRow>
							<NameAndSeniorityWrapper>
								<NameWrapper withNeedAProjectButton={withNeedAProjectButton}>
									<Tooltip text={fullName} useIfTextCut>
										<FixedTypography variant="m-700">
											{fullName || '-'}
										</FixedTypography>
									</Tooltip>{' '}
									{position && (
										<FixedTypography variant="m-400">
											{position}
										</FixedTypography>
									)}
									{isShowSpecialization && (
										<SpecializationName variant="m-600">
											{specializationAndSeniorityValue}
										</SpecializationName>
									)}
								</NameWrapper>
								{withNeedAProjectButton && (
									<NeedProjectContainer>
										<Switch
											checked={needProject}
											onChange={handleToggleChange}
										/>
										<NeedAProjectTypography
											variant="s-400"
											color={theme.palette['text-gray']}
										>
											{NEED_A_PROJECT_TITLE}
										</NeedAProjectTypography>
									</NeedProjectContainer>
								)}
							</NameAndSeniorityWrapper>
							{TeamsChatButtonComponent ? (
								<PersonalInfoHeaderActionsWrapper>
									{TeamsChatButtonComponent}
								</PersonalInfoHeaderActionsWrapper>
							) : null}
						</PersonalInfoTopRow>

						{isShowComments && (
							<CommentSection>
								<CommentContainer
									userId={userId}
									refetch={refetch}
									comments={comments}
								/>
							</CommentSection>
						)}

						{email && (
							<Tooltip
								useIfTextCut
								text={email}
								fallbackPlacements={['bottom-end', 'left']}
							>
								<EmailTypography variant="m-400">{email}</EmailTypography>
							</Tooltip>
						)}
					</PersonalInfoWrapper>
				</InfoWrapper>
			</Loader>
		</Wrapper>
	);
}
