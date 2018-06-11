/**
 * Создает action для GET_EXPLORATION, чтобы сохранить в store список типов обращений
 * @param {APP.ExplorationsList} explorations список типов обращений
 * @returns {{type: string; payload: APP.ExplorationsList}}
 */
export function getExploration(explorations: APP.ExplorationsList) {
    return {
        type: 'GET_EXPLORATION',
        payload: explorations
    }
}