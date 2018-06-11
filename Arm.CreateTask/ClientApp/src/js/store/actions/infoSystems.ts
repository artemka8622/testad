/**
 * Создает action для GET_INFO_SYSTEMS, чтобы сохранить в store список ИС
 * @param {APP.InfoSysProductsList} systems список Информационных Систем
 * @returns {{type: string; payload: APP.InfoSysProductsList}}
 */
export function getInformationSystems(systems: APP.InfoSysProductsList) {
    return {
        type: 'GET_INFO_SYSTEMS',
        payload: systems
    }
}