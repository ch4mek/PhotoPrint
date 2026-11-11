import { useLoadingStatus } from '@mms/mms-ui-library/components/ManagedLoader';
import { useQueryClient } from '@tanstack/react-query';
import { useCallback, useContext, useMemo } from 'react';

import { PersonalDataContext } from '../../context/personalDataContext';
import { TeamboardUserAccessLevel } from '../../types';
import { hasAccess } from '../../utils';

import { InfoDataContainer } from '@/components/InfoDataContainer';
import { Item } from '@/components/InfoDataContainer/Item';
import {
	ContainerWrapper,
	ContentColumn,
	ContentEditWrapper,
} from '@/components/InfoDataContainer/styles';
import {
	TeamboardQueriesKeys,
	UsersQueriesKeys,
} from '@/constants/queriesKeys';
import { useConfiguration } from '@/context/Configuration';
import { useToast } from '@/context/Toast';
import { RegisteredLocationForm } from '@/forms/TeamboardForm/RegisteredLocation';
import { findById } from '@/helpers/findById';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useUpdateRegisteredLocation } from '@/queries/registeredLocation/useUpdateRegisteredLocation';
import { PersonalDataResponse } from '@/types/personalData';
import { RegisteredLocationResponse } from '@/types/registeredLocation';

import { RegisteredLocationFields } from './constants';

export function LocationContainer() {
	const toast = useToast();
	const { id, handleOpen, handleClose, isOpen } = useShowModal();
	const {
		data,
		isFetching,
		showLoader,
		userId,
		accessLevel,
		setPersonalDataChanged,
	} = useContext(PersonalDataContext);
	const {
		configurableValues: { countries },
		persistentValues: { locationTypes },
	} = useConfiguration();

	useLoadingStatus(LocationContainer.name, isFetching || showLoader);

	const queryClient = useQueryClient();

	const updateregisteredLocationMutation = useUpdateRegisteredLocation(
		{
			onSuccess: ({
				currentLocation,
				registeredLocationId,
				registeredLocationName: registeredLocation,
				location: locationName,
			}) => {
				setPersonalDataChanged?.();
				queryClient.setQueryData<PersonalDataResponse>(
					[TeamboardQueriesKeys.personalData, userId],
					(prevData) =>
						({
							...prevData,
							currentLocation,
							registeredLocation,
							registeredLocationId,
							location: locationName,
						} as PersonalDataResponse)
				);
				queryClient.invalidateQueries({
					queryKey: [UsersQueriesKeys.usersTab],
				});
			},
		},
		toast
	);

	const { registeredLocation, currentLocation, location } =
		data || ({} as RegisteredLocationResponse);

	const initialValues = useMemo(() => {
		if (!updateregisteredLocationMutation.data?.registeredLocationName) {
			return {
				currentLocation: data.currentLocation ?? '',
				registeredLocation: data.registeredLocationId
					? findById(countries, data.registeredLocationId) ?? null
					: null,
				location: findById(locationTypes, data.location) ?? null,
			};
		}

		return {
			currentLocation:
				updateregisteredLocationMutation.data.currentLocation ?? '',
			registeredLocation:
				findById(
					countries,
					updateregisteredLocationMutation.data.registeredLocationId
				) ?? null,
			location:
				findById(
					locationTypes,
					updateregisteredLocationMutation.data.location
				) ?? null,
		};
	}, [data, updateregisteredLocationMutation, countries, locationTypes]);

	const handleEditClick = useCallback(() => {
		handleOpen(userId);
	}, [userId]);

	const canEditLocation = hasAccess(accessLevel, [
		TeamboardUserAccessLevel.Full,
		TeamboardUserAccessLevel.LimitedHR,
	]);

	return (
		<>
			<InfoDataContainer
				headerTitle="Location"
				handleEditClick={canEditLocation ? handleEditClick : undefined}
			>
				<ContainerWrapper>
					<ContentEditWrapper>
						<ContentColumn>
							<Item
								nameOfField={RegisteredLocationFields.CURRENT_LOCATION}
								value={currentLocation}
							/>
						</ContentColumn>
						<ContentColumn>
							<Item
								nameOfField={RegisteredLocationFields.REGISTERED_LOCATION}
								value={
									updateregisteredLocationMutation.data
										?.registeredLocationName ?? registeredLocation
								}
							/>
						</ContentColumn>
						<ContentColumn>
							<Item
								nameOfField={RegisteredLocationFields.LOCATION}
								value={
									updateregisteredLocationMutation.data?.location ?? location
								}
							/>
						</ContentColumn>
					</ContentEditWrapper>
				</ContainerWrapper>
			</InfoDataContainer>

			{isOpen && (
				<RegisteredLocationForm
					id={id as number}
					onSubmit={updateregisteredLocationMutation.mutateAsync}
					handleClose={handleClose}
					initialValues={initialValues}
				/>
			)}
		</>
	);
}
