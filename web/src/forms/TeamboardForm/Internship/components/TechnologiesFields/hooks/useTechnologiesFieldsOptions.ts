import { useCallback, useMemo } from 'react';

import { internshipSeniorityOptions } from '../constants';
import { TechnologyFormItem } from '../types';
import { appendSelectedOptions } from '../utils';

import { useInternshipSpecializations } from '@/queries/internship/useInternshipSpecializations';
import { AutoselectOption } from '@/types/common';

export function useTechnologiesFieldsOptions(
	technologies: TechnologyFormItem[]
) {
	const { data } = useInternshipSpecializations();

	const loadedSpecializationOptions = useMemo(
		() =>
			data?.map(({ specializationTemplateId, specializationTemplateName }) => ({
				id: specializationTemplateId,
				name: specializationTemplateName,
			})) ?? [],
		[data]
	);

	const specializationOptions = useMemo(() => {
		const selectedSpecializations = technologies
			.map(({ specialization }) => specialization)
			.filter((specialization): specialization is AutoselectOption =>
				Boolean(specialization)
			);

		return appendSelectedOptions(
			loadedSpecializationOptions,
			selectedSpecializations
		);
	}, [loadedSpecializationOptions, technologies]);

	const hardSkillsBySpecializationId = useMemo(() => {
		const hardSkillsMap = new Map<number, AutoselectOption[]>();

		data?.forEach(({ specializationTemplateId, hardSkills }) => {
			hardSkillsMap.set(
				specializationTemplateId,
				hardSkills.map(({ id, name }) => ({ id, name }))
			);
		});

		return hardSkillsMap;
	}, [data]);

	const getHardSkillOptions = useCallback(
		(specializationId: number) => {
			const loadedHardSkillOptions =
				hardSkillsBySpecializationId.get(specializationId);

			if (loadedHardSkillOptions) {
				return loadedHardSkillOptions;
			}

			const technology = technologies.find(
				({ specialization }) => specialization?.id === specializationId
			);

			return (
				technology?.hardSkills
					.map(({ hardSkill }) => hardSkill)
					.filter(
						(hardSkill): hardSkill is AutoselectOption => hardSkill != null
					) ?? []
			);
		},
		[hardSkillsBySpecializationId, technologies]
	);

	return {
		specializationOptions,
		getHardSkillOptions,
		seniorityOptions: internshipSeniorityOptions,
	};
}
