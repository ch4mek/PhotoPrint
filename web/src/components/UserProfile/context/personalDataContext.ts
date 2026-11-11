import { createContext } from 'react';

import { PersonalDataContextValues } from '../types';

import { Roles } from '@/constants/roles';

const defaultContextValue: PersonalDataContextValues = {
	userId: 0,
	isFetching: true,
	isLoading: true,
	showLoader: true,
	data: {
		fullName: '',
		fullNameRu: null,
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
		resourceGroupName: null,
		performanceReviewDateStatus: null,
		upcomingPerfomanceReview: null,
		location: '',
		registeredLocation: '',
		dateOfBirth: null,
		telegram: null,
		needProject: false,
		registeredLocationId: null,
		resourceGroupId: null,
		currentLocation: null,
		firstName: '',
		lastName: '',
		firstNameRu: null,
		lastNameRu: null,
		patronymicRu: null,
		militaryServiceStatus: null,
		isGraduatedJobPlacement: null,
		linkedIn: null,
		position: Roles.SE,
	},
	refetch: () => {},
};

export const PersonalDataContext =
	createContext<PersonalDataContextValues>(defaultContextValue);
