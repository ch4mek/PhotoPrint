import { useQueryClient } from '@tanstack/react-query';
import { useCallback, useEffect, useState } from 'react';
import { Navigate, useNavigate } from 'react-router-dom';

import { Hexagons } from '@/components/Hexagons';
import { PersonalInfo } from '@/components/PersonalInfo';
import { chiefRoles } from '@/constants/roles';
import { DEFAULT_NAVIGATION_ROUTE, Routes } from '@/constants/routes';
import { useAuth } from '@/context/AuthContext';
import { useToast } from '@/context/Toast';
import { checkIsShowSetProfile } from '@/helpers/checkIsShowSetProfile';
import { setAuthState } from '@/helpers/localStorage';
import { removeExtraSpaces } from '@/helpers/removeExtraSpaces';
import { useAxiosInterCeptors } from '@/hooks/useAxiosInterceptors';
import { useUpdateCurrentUser } from '@/queries/user/useUpdateCurrentUser';
import { GET_USER_INFO_QUERY_KEY } from '@/queries/useUserInfo';
import { UnknownObject } from '@/types/common';

import { SetProfileForm } from './components/ProfileForm';
import { validationSchema, validationSchemaCEO } from './components/schema';
import { ProfileFormValues } from './components/types';
import { CustomAvatar, SetProfilePanel, SetProfileWrapper } from './styles';

export function SetProfile() {
	const userInfo = useAuth();

	const {
		id: userId,
		position,
		fullName,
		firstName,
		lastName,
		email,
		location,
		departmentId,
		token,
		onLogout,
		setUserInfo,
	} = userInfo;

	const queryClient = useQueryClient();
	const navigate = useNavigate();
	const toast = useToast();

	const [isSubmitting, setIsSubmitting] = useState(false);

	useAxiosInterCeptors(token, onLogout);

	const { mutate: updateCurrentUserMutation } = useUpdateCurrentUser(
		{
			onSuccess: async (data) => {
				setAuthState(data);
				setUserInfo((prev) => ({
					...prev,
					...data,
				}));

				await queryClient.setQueryData(
					[GET_USER_INFO_QUERY_KEY],
					(prev: any) => ({
						...prev,
						...data,
					})
				);

				const { location: updatedLocation, departmentId: updatedDepartment } =
					data;

				setIsSubmitting(false);

				if (updatedLocation && (updatedDepartment || departmentId)) {
					navigate(DEFAULT_NAVIGATION_ROUTE);
				}
			},
			onError: () => setIsSubmitting(false),
		},
		toast
	);

	const handleSignIn = useCallback(
		({
			id,
			firstName: customFirstName,
			lastName: customLastName,
			departmentId: profileDepartment,
			location: profilrLocation,
			timeZone,
			avatarFileId,
		}: ProfileFormValues) => {
			const { firstName: firstNameSpaces, lastName: lastNameSpaces } =
				removeExtraSpaces(
					{
						firstName: customFirstName,
						lastName: customLastName,
						location: undefined,
						department: undefined,
					} as UnknownObject,
					(chiefRoles.includes(position)
						? validationSchema
						: validationSchemaCEO) as any
				);
			setIsSubmitting(true);

			updateCurrentUserMutation({
				id,
				firstName: firstNameSpaces as string,
				lastName: lastNameSpaces as string,
				departmentId: profileDepartment,
				location: profilrLocation,
				timeZone,
				avatarFileId,
			});
		},
		[]
	);

	const isShowSetProfile = checkIsShowSetProfile(
		location,
		departmentId,
		position
	);

	useEffect(() => {
		if (!isShowSetProfile) {
			navigate(Routes.root);
		}
	}, [isShowSetProfile]);

	if (!token) {
		return <Navigate to={Routes.signIn} replace state={{ from: location }} />;
	}

	if (!isShowSetProfile) {
		return null;
	}

	return (
		<SetProfileWrapper>
			<Hexagons />
			<SetProfilePanel>
				<PersonalInfo
					isShowLoader={false}
					isNeedWrapperStyles={false}
					position={position}
					seniority={null}
					email={email}
					primarySpecialization={null}
					fullName={fullName}
					isShowComments={false}
					isShowSpecialization={false}
					refetch={() => {}}
					comments={null}
					AvatarComponent={CustomAvatar}
					userId={userId}
					avatarProps={{
						src: '',
						firstName,
						lastName,
					}}
				/>
				<SetProfileForm onSubmit={handleSignIn} isSubmitting={isSubmitting} />
			</SetProfilePanel>
		</SetProfileWrapper>
	);
}
