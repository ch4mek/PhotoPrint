import { createContext } from 'react';

import {
	PersonalDataContextValues,
	TeamboardUserAccessLevel,
	TeamboardUserProfileKind,
} from '../types';

import { Roles } from '@/constants/roles';

const defaultContextValue: PersonalDataContextValues = {
	userId: 0,
	isFetching: true,
	isLoading: true,
	showLoader: true,
	accessLevel: TeamboardUserAccessLevel.None,
	profileKind: TeamboardUserProfileKind.Employee,
	data: {
		fullName: '',
		fullNameRu: null,
		firstName: '',
		lastName: '',
		firstNameRu: null,
		lastNameRu: null,
		patronymicRu: null,
		email: '',
		personalEmail: null,
		personalEmailSecond: null,
		personalPhone: null,
		seniority: null,
		softSkillAverageSeniority: null,
		englishLevel: null,
		comments: null,
		primarySpecialization: null,
		avatarFileId: null,
		departmentId: null,
		departmentFullName: null,
		resourceGroupId: null,
		resourceGroupName: null,
		performanceReviewDateStatus: null,
		upcomingPerfomanceReview: null,
		location: '',
		currentLocation: null,
		registeredLocation: '',
		dateOfBirth: null,
		telegram: null,
		linkedIn: null,
		needProject: false,
		registeredLocationId: null,

		militaryServiceStatus: null,
		isGraduatedJobPlacement: null,
		position: Roles.SE,
	},
	position: Roles.SE,
	refetch: () => {},
};

export const PersonalDataContext =
	createContext<PersonalDataContextValues>(defaultContextValue);
